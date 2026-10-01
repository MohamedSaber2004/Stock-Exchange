using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Repositories.Interfaces;

namespace Stock_Exchange.Application.Features.Auth.Commands.UpdateUserInfo
{
    public class UpdateUserInfoCommandHandler : IRequestHandler<UpdateUserInfoCommand, string>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICountryRepository _countryRepository;
        private readonly IStringLocalizer<Messages> _localizer;

        public UpdateUserInfoCommandHandler(
            UserManager<ApplicationUser> userManager,
            ICurrentUserService currentUserService,
            ICountryRepository countryRepository,
            IStringLocalizer<Messages> localizer)
        {
            _userManager = userManager;
            _currentUserService = currentUserService;
            _countryRepository = countryRepository;
            _localizer = localizer;
        }

        public async Task<string> Handle(UpdateUserInfoCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(_localizer[LocalizationKeys.ExceptionMessages.Unauthorized]);

            var user = await _userManager.FindByIdAsync(_currentUserService.UserId.ToString());
            if (user == null || user.IsDeleted || !user.IsActive)
                throw new NotFoundException(_localizer[LocalizationKeys.ExceptionMessages.NotFound]);

            if (request.CountryId.HasValue)
            {
                var country = await _countryRepository.GetFirstAsync(
                    c => c.Id == request.CountryId.Value && !c.IsDeleted && c.IsActive,
                    cancellationToken);

                if (country == null)
                    throw new NotFoundException(_localizer[LocalizationKeys.CountryMessages.CountryNotFound]);

                user.CountryId = country.Id;
            }

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var trimmedEmail = request.Email.Trim();
                var normalizedEmail = trimmedEmail.ToUpperInvariant();
                var emailExists = await _userManager.Users
                    .IgnoreQueryFilters()
                    .AnyAsync(u => u.Id != user.Id && u.NormalizedEmail == normalizedEmail, cancellationToken);

                if (emailExists)
                    throw new ConflictException(_localizer[LocalizationKeys.AuthMessages.EmailAlreadyExists]);

                user.Email = trimmedEmail;
                user.NormalizedEmail = normalizedEmail;
                user.UserName = trimmedEmail;
                user.NormalizedUserName = normalizedEmail;
            }

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                var cleanPhone = CleanPhoneNumber(request.PhoneNumber);
                var rawPhone = request.PhoneNumber.Trim();

                var phoneExists = await _userManager.Users
                    .IgnoreQueryFilters()
                    .AnyAsync(u => u.Id != user.Id && (u.PhoneNumber == rawPhone || u.PhoneNumber == cleanPhone),
                              cancellationToken);

                if (phoneExists)
                    throw new ConflictException(_localizer[LocalizationKeys.AuthMessages.PhoneNumberAlreadyExists]);

                user.PhoneNumber = cleanPhone;
            }

            if (!string.IsNullOrWhiteSpace(request.FullName))
            {
                user.UpdateFullName(request.FullName);
            }

            if (request.ProfilePictureUrl != null)
            {
                user.UpdateProfilePicture(string.IsNullOrWhiteSpace(request.ProfilePictureUrl) ? null : request.ProfilePictureUrl.Trim());
            }

            if (request.Language.HasValue)
            {
                user.ChangeLanguage(request.Language.Value);
            }

            user.MarkAsUpdated(_currentUserService.UserId.ToString());

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = updateResult.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());

                throw new BadRequestException(errors, LocalizationKeys.ExceptionMessages.BadRequest);
            }

            return _localizer[LocalizationKeys.AuthMessages.UserInfoUpdated];
        }

        private static string CleanPhoneNumber(string phoneNumber)
        {
            return phoneNumber.Trim().Replace(" ", "").Replace("-", "");
        }
    }
}


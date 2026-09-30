using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Repositories.Interfaces;

namespace Stock_Exchange.Application.Features.Auth.Commands.UpdateUserInfo
{
    public class UpdateUserInfoCommandValidator : AbstractValidator<UpdateUserInfoCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICountryRepository _countryRepository;

        public UpdateUserInfoCommandValidator(
            UserManager<ApplicationUser> userManager,
            ICurrentUserService currentUserService,
            ICountryRepository countryRepository)
        {
            _userManager = userManager;
            _currentUserService = currentUserService;
            _countryRepository = countryRepository;

            RuleFor(x => x)
                .Must(_ => _currentUserService.IsAuthenticated && _currentUserService.UserId != Guid.Empty)
                .WithMessage(LocalizationKeys.ExceptionMessages.Unauthorized)
                .DependentRules(() =>
                {
                    RuleFor(x => x)
                        .MustAsync(async (_, cancellationToken) =>
                        {
                            var currentUserId = _currentUserService.UserId;
                            return await _userManager.Users
                                .AsNoTracking()
                                .AnyAsync(u => u.Id == currentUserId && !u.IsDeleted && u.IsActive, cancellationToken);
                        })
                        .WithMessage(LocalizationKeys.AuthMessages.AccountDeactivated);

                    RuleFor(x => x.FullName)
                        .NotEmpty()
                        .WithMessage(LocalizationKeys.AuthMessages.FullNameRequired)
                        .MinimumLength(2)
                        .WithMessage(LocalizationKeys.AuthMessages.FullNameRequired)
                        .MaximumLength(150)
                        .WithMessage(LocalizationKeys.AuthMessages.FullNameRequired)
                        .When(x => x.FullName != null);

                    RuleFor(x => x.CountryId)
                        .NotEmpty()
                        .WithMessage(LocalizationKeys.CountryMessages.CountryNotFound)
                        .MustAsync(async (countryId, cancellationToken) =>
                        {
                            if (!countryId.HasValue || countryId.Value == Guid.Empty)
                                return false;

                            return await _countryRepository.ExistsAsync(
                                c => c.Id == countryId.Value && !c.IsDeleted && c.IsActive,
                                cancellationToken);
                        })
                        .WithMessage(LocalizationKeys.CountryMessages.CountryNotFound)
                        .When(x => x.CountryId.HasValue);

                    RuleFor(x => x.Email)
                        .NotEmpty()
                        .WithMessage(LocalizationKeys.AuthMessages.EmailRequired)
                        .EmailAddress()
                        .WithMessage(LocalizationKeys.AuthMessages.InvalidEmail)
                        .MustAsync(async (command, email, cancellationToken) =>
                        {
                            if (string.IsNullOrWhiteSpace(email))
                                return true;

                            var normalizedEmail = email.Trim().ToUpperInvariant();
                            var currentUserId = _currentUserService.UserId;

                            var isTakenByOther = await _userManager.Users
                                .IgnoreQueryFilters()
                                .AsNoTracking()
                                .AnyAsync(u => u.Id != currentUserId &&
                                               u.NormalizedEmail == normalizedEmail,
                                          cancellationToken);

                            return !isTakenByOther;
                        })
                        .WithMessage(LocalizationKeys.AuthMessages.EmailAlreadyExists)
                        .When(x => x.Email != null);

                    RuleFor(x => x.PhoneNumber)
                        .NotEmpty()
                        .WithMessage(LocalizationKeys.AuthMessages.PhoneNumberRequired)
                        .Matches(@"^[0-9+\-\s()]{6,20}$")
                        .WithMessage(LocalizationKeys.AuthMessages.InvalidPhoneNumber)
                        .MustAsync(async (command, phoneNumber, cancellationToken) =>
                        {
                            if (string.IsNullOrWhiteSpace(phoneNumber))
                                return true;

                            var rawPhone = phoneNumber.Trim();
                            var cleanPhone = CleanPhoneNumber(rawPhone);
                            var currentUserId = _currentUserService.UserId;

                            var isTakenByOther = await _userManager.Users
                                .IgnoreQueryFilters()
                                .AsNoTracking()
                                .AnyAsync(u => u.Id != currentUserId &&
                                               (u.PhoneNumber == rawPhone || u.PhoneNumber == cleanPhone),
                                          cancellationToken);

                            return !isTakenByOther;
                        })
                        .WithMessage(LocalizationKeys.AuthMessages.PhoneNumberAlreadyExists)
                        .When(x => x.PhoneNumber != null);

                    RuleFor(x => x.Language)
                        .IsInEnum()
                        .When(x => x.Language.HasValue);
                });
        }

        private static string CleanPhoneNumber(string phoneNumber)
        {
            return phoneNumber.Trim().Replace(" ", "").Replace("-", "");
        }
    }
}

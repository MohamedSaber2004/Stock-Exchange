using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Options;
using Stock_Exchange.Application.Features.Auth.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Enums;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Application.Features.Auth.Commands.Register
{
    public sealed class SignupCommandHandler : IRequestHandler<SignupCommand, AuthResponseDto>
    {
        private const int ProfilePicturePlace = 0;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ICountryRepository _countryRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUserRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtSettings _jwtSettings;
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly IImageValidator _imageValidator;

        public SignupCommandHandler(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            ICountryRepository countryRepository,
            IJwtTokenService jwtTokenService,
            IUserRefreshTokenRepository refreshTokenRepository,
            IUnitOfWork unitOfWork,
            IOptions<JwtSettings> jwtSettings,
            IStringLocalizer<Messages> localizer,
            IImageValidator imageValidator)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _countryRepository = countryRepository;
            _jwtTokenService = jwtTokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
            _jwtSettings = jwtSettings.Value;
            _localizer = localizer;
            _imageValidator = imageValidator;
        }

        public async Task<AuthResponseDto> Handle(SignupCommand request, CancellationToken cancellationToken)
        {
            var country = await _countryRepository.GetFirstAsync(
                c => c.Id == request.CountryId && !c.IsDeleted && c.IsActive,
                cancellationToken);

            if (country == null)
                throw new NotFoundException(_localizer[LocalizationKeys.CountryMessages.CountryNotFound]);

            var normalizedEmail = request.Email.Trim().ToUpperInvariant();
            var activeEmailExists = await _userManager.Users
                .AnyAsync(u => !u.IsDeleted && u.NormalizedEmail == normalizedEmail, cancellationToken);

            if (activeEmailExists)
                throw new ConflictException(_localizer[LocalizationKeys.AuthMessages.EmailAlreadyExists]);

            var cleanPhone = CleanPhoneNumber(request.PhoneNumber);
            var rawPhone = request.PhoneNumber.Trim();

            var activePhoneExists = await _userManager.Users
                .AnyAsync(u => !u.IsDeleted && (u.PhoneNumber == rawPhone || u.PhoneNumber == cleanPhone),
                          cancellationToken);

            if (activePhoneExists)
                throw new ConflictException(_localizer[LocalizationKeys.AuthMessages.PhoneNumberAlreadyExists]);

            var deletedUser = await _userManager.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.IsDeleted && u.NormalizedEmail == normalizedEmail, cancellationToken);

            ApplicationUser user;
            if (deletedUser != null)
            {
                user = deletedUser;
                user.UserName = request.Email.Trim();
                user.NormalizedUserName = normalizedEmail;
                user.Email = request.Email.Trim();
                user.NormalizedEmail = normalizedEmail;
                user.PhoneNumber = cleanPhone;
                user.CountryId = country.Id;
                user.UpdateFullName(request.FullName);

                if (!string.IsNullOrWhiteSpace(user.ProfilePictureUrl) &&
                    !user.ProfilePictureUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                    !user.ProfilePictureUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    await _imageValidator.DeleteImage(user.ProfilePictureUrl, ProfilePicturePlace);
                }
                user.UpdateProfilePicture(null);

                user.Activate();
                user.IncrementTokenVersion();
                user.MarkAsUpdated(user.Email ?? "System");

                if (await _userManager.HasPasswordAsync(user))
                {
                    var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetResult = await _userManager.ResetPasswordAsync(user, resetToken, request.Password);
                    if (!resetResult.Succeeded)
                    {
                        var errors = resetResult.Errors
                            .GroupBy(e => e.Code)
                            .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());

                        throw new BadRequestException(
                            errors,
                            LocalizationKeys.AuthMessages.UserCreationFailed);
                    }
                }
                else
                {
                    var addPasswordResult = await _userManager.AddPasswordAsync(user, request.Password);
                    if (!addPasswordResult.Succeeded)
                    {
                        var errors = addPasswordResult.Errors
                            .GroupBy(e => e.Code)
                            .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());

                        throw new BadRequestException(
                            errors,
                            LocalizationKeys.AuthMessages.UserCreationFailed);
                    }
                }

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    var errors = updateResult.Errors
                        .GroupBy(e => e.Code)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());

                    throw new BadRequestException(
                        errors,
                        LocalizationKeys.AuthMessages.UserCreationFailed);
                }

                var oldTokens = await _refreshTokenRepository
                    .GetAllAsync(x => x.UserId == user.Id && !x.IsRevoked)
                    .ToListAsync(cancellationToken);

                foreach (var token in oldTokens)
                {
                    token.Revoke();
                }

                await _unitOfWork.SaveChangesAsync();
            }
            else
            {
                user = new ApplicationUser
                {
                    UserName = request.Email.Trim(),
                    Email = request.Email.Trim(),
                    PhoneNumber = cleanPhone,
                    CountryId = country.Id
                };
                user.UpdateFullName(request.FullName);

                var createResult = await _userManager.CreateAsync(user, request.Password);
                if (!createResult.Succeeded)
                {
                    var errors = createResult.Errors
                        .GroupBy(e => e.Code)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());

                    throw new BadRequestException(
                        errors,
                        LocalizationKeys.AuthMessages.UserCreationFailed);
                }
            }

            var customerRole = UserType.Customer.ToString();
            if (!await _roleManager.RoleExistsAsync(customerRole))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(customerRole));
            }

            var existingRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in existingRoles)
            {
                if (!string.Equals(role, customerRole, StringComparison.OrdinalIgnoreCase))
                {
                    await _userManager.RemoveFromRoleAsync(user, role);
                }
            }

            if (!await _userManager.IsInRoleAsync(user, customerRole))
            {
                await _userManager.AddToRoleAsync(user, customerRole);
            }

            var roles = new List<string> { customerRole };
            var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);

            var refreshToken = _jwtTokenService.GenerateRefreshToken(user);
            var userRefreshToken = UserRefreshToken.Create(
                user.Id,
                refreshToken,
                DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays));

            await _refreshTokenRepository.AddAsync(userRefreshToken);
            await _unitOfWork.SaveChangesAsync();

            return new AuthResponseDto(
                accessToken,
                refreshToken,
                user.FullName,
                user.Email!,
                user.PhoneNumber ?? string.Empty,
                customerRole,
                user.Id,
                !string.IsNullOrWhiteSpace(user.ProfilePictureUrl) ? user.ProfilePictureUrl : ApplicationUser.DefaultProfilePicture);
        }

        private static string CleanPhoneNumber(string phoneNumber)
        {
            return phoneNumber.Trim().Replace(" ", "").Replace("-", "");
        }
    }
}

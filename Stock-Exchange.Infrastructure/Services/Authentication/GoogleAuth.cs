using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Common.Options;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Infrastructure.Services.Authentication
{
    public class GoogleAuth : IGoogleAuth
    {
        private const string GoogleLoginProvider = "Google";

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly GoogleIdTokenValidator _idTokenValidator;
        private readonly GoogleAuthSettings _googleAuthSettings;
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly ILogger<GoogleAuth> _logger;

        public GoogleAuth(
            UserManager<ApplicationUser> userManager,
            GoogleIdTokenValidator idTokenValidator,
            IOptions<GoogleAuthSettings> googleAuthSettings,
            IStringLocalizer<Messages> localizer,
            ILogger<GoogleAuth> logger)
        {
            _userManager = userManager;
            _idTokenValidator = idTokenValidator;
            _googleAuthSettings = googleAuthSettings.Value;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<GoogleUserProfile?> ValidateGoogleTokenAsync(string idToken, string correlationId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(idToken))
                return null;

            if (string.IsNullOrWhiteSpace(_googleAuthSettings.WebClientId)
                && (_googleAuthSettings.WebClientIds is null || _googleAuthSettings.WebClientIds.Length == 0))
            {
                throw new ServiceUnavailableException(_localizer[LocalizationKeys.ExceptionMessages.GoogleAuthNotConfigured]);
            }

            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var token = await _idTokenValidator.ValidateAsync(idToken, cancellationToken);

                return new GoogleUserProfile(
                    Subject: token.Subject,
                    Email: ReadClaim(token, "email"),
                    Name: ReadClaim(token, "name"),
                    Picture: ReadClaim(token, "picture"),
                    EmailVerified: ReadEmailVerified(token));
            }
            catch (Exception)
            {
                throw new ServiceUnavailableException(_localizer[LocalizationKeys.ExceptionMessages.GoogleAuthValidationUnavailable]);
            }
        }

        public async Task LinkGoogleAccountIfNeeded(ApplicationUser user, GoogleUserProfile profile, string correlationId)
        {
            if (string.IsNullOrWhiteSpace(profile.Subject))
                throw new UnAuthorizedException(_localizer[LocalizationKeys.AuthMessages.InvalidGoogleToken]);

            var logins = await _userManager.GetLoginsAsync(user);
            var existingGoogleLogin = logins.FirstOrDefault(l =>
                string.Equals(l.LoginProvider, GoogleLoginProvider, StringComparison.OrdinalIgnoreCase));

            if (existingGoogleLogin is not null)
            {
                if (!string.Equals(existingGoogleLogin.ProviderKey, profile.Subject, StringComparison.Ordinal))
                {
                    throw new ConflictException(_localizer[LocalizationKeys.AuthMessages.GoogleAccountAlreadyLinked]);
                }

                user.LinkGoogleAccount(profile.Subject);
                return;
            }

            var addLoginResult = await _userManager.AddLoginAsync(
                user,
                new UserLoginInfo(GoogleLoginProvider, profile.Subject, GoogleLoginProvider));

            if (!addLoginResult.Succeeded)
            {
                throw new BadRequestException(ToErrors(addLoginResult), _localizer[LocalizationKeys.AuthMessages.GoogleAccountLinkFailed]);
            }

            user.LinkGoogleAccount(profile.Subject);

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                throw new BadRequestException(ToErrors(updateResult), _localizer[LocalizationKeys.AuthMessages.GoogleAccountLinkFailed]);
            }
        }

        public async Task UpdateUserInfoFromGoogle(ApplicationUser user, GoogleUserProfile profile, string correlationId)
        {
            if (string.IsNullOrWhiteSpace(profile.Name)
                || string.Equals(user.FullName, profile.Name.Trim(), StringComparison.Ordinal))
            {
                return;
            }

            user.UpdateFullName(profile.Name);

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                throw new BadRequestException(ToErrors(updateResult), _localizer[LocalizationKeys.AuthMessages.GoogleProfileUpdateFailed]);
            }
        }

        private static string? ReadClaim(JsonWebToken token, string claimName) =>
            token.Claims.FirstOrDefault(c => c.Type == claimName)?.Value;

        private static bool ReadEmailVerified(JsonWebToken token)
        {
            var claim = token.Claims.FirstOrDefault(c => c.Type == "email_verified");
            if (claim is null)
                return false;

            return bool.TryParse(claim.Value, out var verified) && verified;
        }

        private static IDictionary<string, string[]> ToErrors(IdentityResult result) =>
            result.Errors
                .GroupBy(e => e.Code)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());
    }
}

using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Options;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Infrastructure.Services.Authentication
{
    public class GoogleAuth : IGoogleAuth
    {
        private const string GoogleLoginProvider = "Google";

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly GoogleAuthSettings _googleAuthSettings;
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly ILogger<GoogleAuth> _logger;

        public GoogleAuth(
            UserManager<ApplicationUser> userManager,
            IOptions<GoogleAuthSettings> googleAuthSettings,
            IStringLocalizer<Messages> localizer,
            ILogger<GoogleAuth> logger)
        {
            _userManager = userManager;
            _googleAuthSettings = googleAuthSettings.Value;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<GoogleJsonWebSignature.Payload?> ValidateGoogleTokenAsync(string idToken, string correlationId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(idToken))
                return null;

            if (string.IsNullOrWhiteSpace(_googleAuthSettings.WebClientId))
                throw new ServiceUnavailableException(_localizer[LocalizationKeys.ExceptionMessages.GoogleAuthNotConfigured]);

            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _googleAuthSettings.WebClientId }
                };

                // Google.Apis.Auth 1.76.0 exposes no CancellationToken overload and pins the
                // issuer to Google's own signing certificates internally.
                return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            }
            catch (InvalidJwtException)
            {
                return null;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                // The SDK also throws raw Newtonsoft/Base64/Format errors for malformed token
                // segments, and can fail when the Google certificate endpoint is unreachable.
                // None of those are trustworthy tokens, and none may surface as a 500 (which
                // would return a stack trace to the caller outside Production). The type is
                // logged so genuine outages stay diagnosable.
                _logger.LogWarning(
                    ex,
                    "Google ID token validation failed unexpectedly ({ExceptionType}). CorrelationId: {CorrelationId}",
                    ex.GetType().Name, correlationId);

                return null;
            }
        }

        public async Task LinkGoogleAccountIfNeeded(ApplicationUser user, GoogleJsonWebSignature.Payload payload, string correlationId)
        {
            if (string.IsNullOrWhiteSpace(payload.Subject))
                throw new UnAuthorizedException(_localizer[LocalizationKeys.AuthMessages.InvalidGoogleToken]);

            var logins = await _userManager.GetLoginsAsync(user);
            var existingGoogleLogin = logins.FirstOrDefault(l =>
                string.Equals(l.LoginProvider, GoogleLoginProvider, StringComparison.OrdinalIgnoreCase));

            if (existingGoogleLogin is not null)
            {
                if (!string.Equals(existingGoogleLogin.ProviderKey, payload.Subject, StringComparison.Ordinal))
                {
                    _logger.LogWarning(
                        "Google sign-in rejected: user {UserId} is already linked to a different Google subject. CorrelationId: {CorrelationId}",
                        user.Id, correlationId);

                    throw new ConflictException(_localizer[LocalizationKeys.AuthMessages.GoogleAccountAlreadyLinked]);
                }

                user.LinkGoogleAccount(payload.Subject);
                return;
            }

            var addLoginResult = await _userManager.AddLoginAsync(
                user,
                new UserLoginInfo(GoogleLoginProvider, payload.Subject, GoogleLoginProvider));

            if (!addLoginResult.Succeeded)
            {
                _logger.LogWarning(
                    "Failed to add Google login for user {UserId}: {Errors}. CorrelationId: {CorrelationId}",
                    user.Id, string.Join(", ", addLoginResult.Errors.Select(e => e.Code)), correlationId);

                throw new BadRequestException(ToErrors(addLoginResult), _localizer[LocalizationKeys.AuthMessages.GoogleAccountLinkFailed]);
            }

            user.LinkGoogleAccount(payload.Subject);

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                _logger.LogWarning(
                    "Failed to persist Google user id for user {UserId}: {Errors}. CorrelationId: {CorrelationId}",
                    user.Id, string.Join(", ", updateResult.Errors.Select(e => e.Code)), correlationId);

                throw new BadRequestException(ToErrors(updateResult), _localizer[LocalizationKeys.AuthMessages.GoogleAccountLinkFailed]);
            }
        }

        // Only the name is synced here. ProfilePictureUrl deliberately holds a locally stored
        // file name (not the Google URL), so the caller uploads payload.Picture via
        // UploadFileCommand and assigns the returned name before the response is built.
        public async Task UpdateUserInfoFromGoogle(ApplicationUser user, GoogleJsonWebSignature.Payload payload, string correlationId)
        {
            var needsUpdate = false;

            if (!string.IsNullOrWhiteSpace(payload.Name)
                && !string.Equals(user.FullName, payload.Name.Trim(), StringComparison.Ordinal))
            {
                user.UpdateFullName(payload.Name);
                needsUpdate = true;
            }

            if (!needsUpdate)
                return;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                _logger.LogWarning(
                    "Failed to sync Google profile for user {UserId}: {Errors}. CorrelationId: {CorrelationId}",
                    user.Id, string.Join(", ", updateResult.Errors.Select(e => e.Code)), correlationId);

                throw new BadRequestException(ToErrors(updateResult), _localizer[LocalizationKeys.AuthMessages.GoogleProfileUpdateFailed]);
            }
        }

        private static IDictionary<string, string[]> ToErrors(IdentityResult result) =>
            result.Errors
                .GroupBy(e => e.Code)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());
    }
}

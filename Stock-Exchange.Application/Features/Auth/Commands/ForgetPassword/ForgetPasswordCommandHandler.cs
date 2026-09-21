using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Options;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Enums;
using System.Security.Cryptography;

namespace Stock_Exchange.Application.Features.Auth.Commands.ForgetPassword
{
    public sealed class ForgetPasswordCommandHandler : IRequestHandler<ForgetPasswordCommand, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<ForgetPasswordCommandHandler> _logger;

        public ForgetPasswordCommandHandler(
            UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            IOptions<EmailSettings> emailSettings,
            ILogger<ForgetPasswordCommandHandler> logger)
        {
            _userManager = userManager;
            _emailService = emailService;
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task<bool> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
                throw new NotFoundException(LocalizationKeys.ExceptionMessages.NotFound);

            if (user.IsDeleted)
                throw new ForbiddenException(LocalizationKeys.AuthMessages.AccountDeleted);

            if (!user.IsActive)
                throw new ForbiddenException(LocalizationKeys.AuthMessages.AccountDeactivated);

            var otpCode = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
            var expiryMinutes = _emailSettings.VerificationCodeExpiryMinutes > 0
                ? _emailSettings.VerificationCodeExpiryMinutes
                : 10;
            var expiryTime = DateTime.Now.AddMinutes(expiryMinutes);

            user.SetVerificationCode(otpCode, expiryTime);

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = updateResult.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());

                throw new BadRequestException(errors, LocalizationKeys.ExceptionMessages.BadRequest);
            }

            var languageCode = user.Language.ToString();
            var templateName = user.Language == Language.ar ? "ForgetPassword_ar" : "ForgetPassword_en";
            var subject = JsonLocalizationProvider.GetLocalizedString(LocalizationKeys.EmailMessages.ResetPasswordSubject, languageCode);

            var placeholders = new Dictionary<string, string>
            {
                { "Name", user.FullName },
                { "Code", otpCode },
                { "ExpiryMinutes", expiryMinutes.ToString() }
            };

            _logger.LogInformation("Password reset OTP generated for {Email}: {OtpCode} (Expires: {ExpiryTime})", user.Email, otpCode, expiryTime);

            try
            {
                await _emailService.SendTemplateEmailAsync(
                    user.Email!,
                    subject,
                    templateName,
                    placeholders,
                    cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deliver password reset email to {Email}. Generated OTP was: {OtpCode}", user.Email, otpCode);
                throw;
            }

            return true;
        }
    }
}

using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Options;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Application.Features.Auth.Commands.VerifyOtp
{
    public sealed class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, string>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EmailSettings _emailSettings;

        public VerifyOtpCommandHandler(
            UserManager<ApplicationUser> userManager,
            IOptions<EmailSettings> emailSettings)
        {
            _userManager = userManager;
            _emailSettings = emailSettings.Value;
        }

        public async Task<string> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
                throw new NotFoundException(LocalizationKeys.ExceptionMessages.NotFound);

            if (user.IsDeleted)
                throw new ForbiddenException(LocalizationKeys.AuthMessages.AccountDeleted);

            if (!user.IsActive)
                throw new ForbiddenException(LocalizationKeys.AuthMessages.AccountDeactivated);

            if (string.IsNullOrWhiteSpace(user.VerificationCode) || user.VerificationCode != request.OtpCode)
                throw new BadRequestException(LocalizationKeys.AuthMessages.InvalidVerificationCode);

            if (user.VerificationCodeExpiry == null || user.VerificationCodeExpiry < DateTime.Now)
                throw new BadRequestException(LocalizationKeys.AuthMessages.VerificationCodeExpired);

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            var expiryMinutes = _emailSettings.VerificationCodeExpiryMinutes > 0
                ? _emailSettings.VerificationCodeExpiryMinutes
                : 10;
            var expiryTime = DateTime.Now.AddMinutes(expiryMinutes);

            user.SetPasswordResetToken(resetToken, expiryTime);

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = updateResult.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());

                throw new BadRequestException(errors, LocalizationKeys.ExceptionMessages.BadRequest);
            }

            return resetToken;
        }
    }
}

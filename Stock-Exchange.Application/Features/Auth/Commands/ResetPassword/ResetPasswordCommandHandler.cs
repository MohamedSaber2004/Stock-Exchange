using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Application.Features.Auth.Commands.ResetPassword
{
    public sealed class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ResetPasswordCommandHandler(
            UserManager<ApplicationUser> userManager,
            IUserRefreshTokenRepository refreshTokenRepository,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
                throw new NotFoundException(LocalizationKeys.ExceptionMessages.NotFound);

            if (user.IsDeleted)
                throw new ForbiddenException(LocalizationKeys.AuthMessages.AccountDeleted);

            if (!user.IsActive)
                throw new ForbiddenException(LocalizationKeys.AuthMessages.AccountDeactivated);

            var inputCode = request.OtpCode?.Trim() ?? string.Empty;
            string tokenToUse;

            if (!string.IsNullOrWhiteSpace(user.VerificationCode) && user.VerificationCode == inputCode)
            {
                if (user.VerificationCodeExpiry == null || user.VerificationCodeExpiry < DateTime.Now)
                    throw new BadRequestException(LocalizationKeys.AuthMessages.VerificationCodeExpired);

                if (!string.IsNullOrWhiteSpace(user.PasswordResetToken) &&
                    (user.PasswordResetTokenExpiry == null || user.PasswordResetTokenExpiry >= DateTime.Now))
                {
                    tokenToUse = user.PasswordResetToken;
                }
                else
                {
                    tokenToUse = await _userManager.GeneratePasswordResetTokenAsync(user);
                }
            }
            else if (!string.IsNullOrWhiteSpace(user.PasswordResetToken) && user.PasswordResetToken == inputCode)
            {
                if (user.PasswordResetTokenExpiry != null && user.PasswordResetTokenExpiry < DateTime.Now)
                    throw new BadRequestException(LocalizationKeys.AuthMessages.ResetTokenExpired);

                tokenToUse = user.PasswordResetToken;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(user.VerificationCode) &&
                    user.VerificationCodeExpiry != null &&
                    user.VerificationCodeExpiry < DateTime.Now)
                {
                    throw new BadRequestException(LocalizationKeys.AuthMessages.VerificationCodeExpired);
                }

                if (!string.IsNullOrWhiteSpace(user.PasswordResetToken) &&
                    user.PasswordResetTokenExpiry != null &&
                    user.PasswordResetTokenExpiry < DateTime.Now)
                {
                    throw new BadRequestException(LocalizationKeys.AuthMessages.ResetTokenExpired);
                }

                if (inputCode.Length == 6 && inputCode.All(char.IsDigit))
                {
                    throw new BadRequestException(LocalizationKeys.AuthMessages.InvalidVerificationCode);
                }

                throw new BadRequestException(LocalizationKeys.AuthMessages.InvalidResetToken);
            }

            var resetResult = await _userManager.ResetPasswordAsync(user, tokenToUse, request.NewPassword);
            if (!resetResult.Succeeded)
            {
                var errors = resetResult.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());

                throw new BadRequestException(errors, LocalizationKeys.AuthMessages.PasswordResetFailed);
            }

            var activeTokens = await _refreshTokenRepository
                .GetAllAsync(x => x.UserId == user.Id && !x.IsRevoked)
                .ToListAsync(cancellationToken);
            foreach (var token in activeTokens)
            {
                token.Revoke();
            }
            await _unitOfWork.SaveChangesAsync();

            user.ClearPasswordResetToken();
            user.ClearVerificationCode();
            // A password change must kill existing sessions: invalidate all
            // previously issued access tokens alongside the revoked refresh tokens.
            user.IncrementTokenVersion();
            user.MarkAsUpdated(user.Email ?? "System");

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = updateResult.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());

                throw new BadRequestException(errors, LocalizationKeys.ExceptionMessages.BadRequest);
            }

            return true;
        }
    }
}

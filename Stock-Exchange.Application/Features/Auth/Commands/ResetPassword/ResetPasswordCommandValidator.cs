using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ResetPasswordCommandValidator(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(LocalizationKeys.AuthMessages.EmailRequired)
                .EmailAddress()
                .WithMessage(LocalizationKeys.AuthMessages.InvalidEmail)
                .MustAsync(EmailExists)
                .WithMessage(LocalizationKeys.ExceptionMessages.NotFound);

            RuleFor(x => x.OtpCode)
                .NotEmpty()
                .WithMessage(LocalizationKeys.AuthMessages.ResetTokenRequired);

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage(LocalizationKeys.AuthMessages.PasswordRequired);

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .WithMessage(LocalizationKeys.AuthMessages.ConfirmPasswordRequired)
                .Equal(x => x.NewPassword)
                .WithMessage(LocalizationKeys.AuthMessages.PasswordsDoNotMatch);
        }

        private async Task<bool> EmailExists(string email, CancellationToken cancellationToken)
        {
            return await _userManager.Users.AnyAsync(u => u.Email == email && !u.IsDeleted, cancellationToken);
        }
    }
}

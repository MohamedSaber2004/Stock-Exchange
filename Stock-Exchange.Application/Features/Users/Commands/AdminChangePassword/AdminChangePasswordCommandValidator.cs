using FluentValidation;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.Users.Commands.AdminChangePassword
{
    public class AdminChangePasswordCommandValidator : AbstractValidator<AdminChangePasswordCommand>
    {
        public AdminChangePasswordCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty();

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage(LocalizationKeys.AuthMessages.PasswordRequired)
                .MinimumLength(6);

            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty()
                .WithMessage(LocalizationKeys.AuthMessages.ConfirmPasswordRequired)
                .Equal(x => x.NewPassword)
                .WithMessage(LocalizationKeys.AuthMessages.PasswordsDoNotMatch);
        }
    }
}

using FluentValidation;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.Auth.Commands.ChangePassword
{
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty()
                .WithMessage(LocalizationKeys.AuthMessages.CurrentPasswordRequired);

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage(LocalizationKeys.AuthMessages.PasswordRequired)
                .NotEqual(x => x.CurrentPassword)
                .WithMessage(LocalizationKeys.AuthMessages.NewPasswordCannotBeOldPassword);

            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty()
                .WithMessage(LocalizationKeys.AuthMessages.ConfirmPasswordRequired)
                .Equal(x => x.NewPassword)
                .WithMessage(LocalizationKeys.AuthMessages.PasswordsDoNotMatch);
        }
    }
}

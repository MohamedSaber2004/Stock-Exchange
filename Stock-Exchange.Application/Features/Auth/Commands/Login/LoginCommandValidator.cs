using FluentValidation;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.Auth.Commands.Login
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(LocalizationKeys.AuthMessages.EmailRequired)
                .EmailAddress()
                .WithMessage(LocalizationKeys.AuthMessages.InvalidEmail);

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage(LocalizationKeys.AuthMessages.PasswordRequired);
        }
    }
}

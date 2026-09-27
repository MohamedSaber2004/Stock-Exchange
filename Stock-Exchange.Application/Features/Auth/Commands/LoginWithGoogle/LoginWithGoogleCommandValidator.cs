using FluentValidation;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.Auth.Commands.LoginWithGoogle
{
    public class LoginWithGoogleCommandValidator : AbstractValidator<LoginWithGoogleCommand>
    {

        public LoginWithGoogleCommandValidator()
        {
            RuleFor(x => x.IdToken)
                .NotEmpty()
                .WithMessage(LocalizationKeys.AuthMessages.GoogleIdTokenRequired);
        }
    }
}

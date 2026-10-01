using FluentValidation;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.Countries.Commands.UpdateCountry
{
    public class UpdateCountryCommandValidator : AbstractValidator<UpdateCountryCommand>
    {
        public UpdateCountryCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();

            RuleFor(x => x.CountryArName)
                .NotEmpty().WithMessage(LocalizationKeys.CountryMessages.CountryArNameRequired)
                .MaximumLength(100);

            RuleFor(x => x.CountryEnName)
                .NotEmpty().WithMessage(LocalizationKeys.CountryMessages.CountryEnNameRequired)
                .MaximumLength(100);

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage(LocalizationKeys.CountryMessages.CountryCodeRequired)
                .MaximumLength(10);
        }
    }
}

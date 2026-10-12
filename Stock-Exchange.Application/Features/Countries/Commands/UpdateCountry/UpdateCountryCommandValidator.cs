using FluentValidation;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.Countries.Commands.UpdateCountry
{
    public class UpdateCountryCommandValidator : AbstractValidator<UpdateCountryCommand>
    {
        public UpdateCountryCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage(LocalizationKeys.CountryMessages.CountryIdRequired);

            RuleFor(x => x.CountryArName)
                .NotEmpty().WithMessage(LocalizationKeys.CountryMessages.CountryArNameRequired)
                .MaximumLength(100).WithMessage(LocalizationKeys.CountryMessages.CountryArNameTooLong);

            RuleFor(x => x.CountryEnName)
                .NotEmpty().WithMessage(LocalizationKeys.CountryMessages.CountryEnNameRequired)
                .MaximumLength(100).WithMessage(LocalizationKeys.CountryMessages.CountryEnNameTooLong);

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage(LocalizationKeys.CountryMessages.CountryCodeRequired)
                .MaximumLength(10).WithMessage(LocalizationKeys.CountryMessages.CountryCodeTooLong);
        }
    }
}

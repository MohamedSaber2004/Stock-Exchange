using FluentValidation;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.Countries.Commands.DeleteCountry
{
    public class DeleteCountryCommandValidator : AbstractValidator<DeleteCountryCommand>
    {
        public DeleteCountryCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage(LocalizationKeys.CountryMessages.CountryIdRequired);
        }
    }
}

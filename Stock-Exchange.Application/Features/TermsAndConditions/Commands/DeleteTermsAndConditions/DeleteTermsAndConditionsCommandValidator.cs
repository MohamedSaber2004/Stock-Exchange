using FluentValidation;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.TermsAndConditions.Commands.DeleteTermsAndConditions
{
    public class DeleteTermsAndConditionsCommandValidator : AbstractValidator<DeleteTermsAndConditionsCommand>
    {
        public DeleteTermsAndConditionsCommandValidator()
        {
            RuleFor(x => x.Id)
                .Must(id => !id.HasValue || id.Value != Guid.Empty)
                .WithMessage(LocalizationKeys.TermsAndConditionsMessages.TermsAndConditionsIdRequired);
        }
    }
}

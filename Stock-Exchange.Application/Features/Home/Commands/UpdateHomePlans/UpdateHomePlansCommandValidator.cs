using FluentValidation;
using Stock_Exchange.Application.Features.Home.DTOs;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.Home.Commands.UpdateHomePlans
{
    public class UpdateHomePlansCommandValidator : AbstractValidator<UpdateHomePlansCommand>
    {
        public UpdateHomePlansCommandValidator()
        {
            RuleForEach(x => x.Items).SetValidator(new HomePlanItemRequestValidator());
        }
    }

    public class HomePlanItemRequestValidator : AbstractValidator<HomePlanItemRequest>
    {
        public HomePlanItemRequestValidator()
        {
            RuleFor(x => x.NameEn)
                .NotEmpty().WithMessage(LocalizationKeys.HomeMessages.HomePlanNameEnRequired)
                .MaximumLength(200).WithMessage(LocalizationKeys.HomeMessages.HomePlanNameEnTooLong);

            RuleFor(x => x.NameAr)
                .NotEmpty().WithMessage(LocalizationKeys.HomeMessages.HomePlanNameArRequired)
                .MaximumLength(200).WithMessage(LocalizationKeys.HomeMessages.HomePlanNameArTooLong);

            RuleFor(x => x.PriceEgp)
                .GreaterThanOrEqualTo(0).WithMessage(LocalizationKeys.HomeMessages.HomePlanPriceInvalid);

            RuleForEach(x => x.Features).SetValidator(new HomePlanFeatureItemRequestValidator());
        }
    }

    public class HomePlanFeatureItemRequestValidator : AbstractValidator<HomePlanFeatureItemRequest>
    {
        public HomePlanFeatureItemRequestValidator()
        {
            RuleFor(x => x.TextEn)
                .NotEmpty().WithMessage(LocalizationKeys.HomeMessages.HomePlanFeatureTextEnRequired)
                .MaximumLength(500).WithMessage(LocalizationKeys.HomeMessages.HomePlanFeatureTextEnTooLong);

            RuleFor(x => x.TextAr)
                .NotEmpty().WithMessage(LocalizationKeys.HomeMessages.HomePlanFeatureTextArRequired)
                .MaximumLength(500).WithMessage(LocalizationKeys.HomeMessages.HomePlanFeatureTextArTooLong);
        }
    }
}

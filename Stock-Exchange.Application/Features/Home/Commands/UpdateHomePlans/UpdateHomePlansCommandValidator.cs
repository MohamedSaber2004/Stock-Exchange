using FluentValidation;
using Stock_Exchange.Application.Features.Home.DTOs;

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
                .NotEmpty().WithMessage("Plan English name is required.")
                .MaximumLength(200).WithMessage("Plan English name must not exceed 200 characters.");

            RuleFor(x => x.NameAr)
                .NotEmpty().WithMessage("Plan Arabic name is required.")
                .MaximumLength(200).WithMessage("Plan Arabic name must not exceed 200 characters.");

            RuleFor(x => x.PriceEgp)
                .GreaterThanOrEqualTo(0).WithMessage("Plan price must be non-negative.");

            RuleForEach(x => x.Features).SetValidator(new HomePlanFeatureItemRequestValidator());
        }
    }

    public class HomePlanFeatureItemRequestValidator : AbstractValidator<HomePlanFeatureItemRequest>
    {
        public HomePlanFeatureItemRequestValidator()
        {
            RuleFor(x => x.TextEn)
                .NotEmpty().WithMessage("Plan feature English text is required.")
                .MaximumLength(500).WithMessage("Plan feature English text must not exceed 500 characters.");

            RuleFor(x => x.TextAr)
                .NotEmpty().WithMessage("Plan feature Arabic text is required.")
                .MaximumLength(500).WithMessage("Plan feature Arabic text must not exceed 500 characters.");
        }
    }
}

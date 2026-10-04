using FluentValidation;

namespace Stock_Exchange.Application.Features.Experts.Commands.AddExpert
{
    public class AddExpertCommandValidator : AbstractValidator<AddExpertCommand>
    {
        public AddExpertCommandValidator()
        {
            RuleFor(x => x.FullNameEn).NotEmpty().WithMessage("Full name in English is required").MaximumLength(200);
            RuleFor(x => x.FullNameAr).NotEmpty().WithMessage("Full name in Arabic is required").MaximumLength(200);
            RuleFor(x => x.TitleEn).NotEmpty().WithMessage("Title/Specialty in English is required").MaximumLength(200);
            RuleFor(x => x.TitleAr).NotEmpty().WithMessage("Title/Specialty in Arabic is required").MaximumLength(200);
            RuleFor(x => x.AvatarUrl).MaximumLength(1024);
            RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0);
        }
    }
}

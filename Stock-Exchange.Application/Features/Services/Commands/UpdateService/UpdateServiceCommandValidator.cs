using FluentValidation;

namespace Stock_Exchange.Application.Features.Services.Commands.UpdateService
{
    public class UpdateServiceCommandValidator : AbstractValidator<UpdateServiceCommand>
    {
        public UpdateServiceCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required");
            RuleFor(x => x.TitleEn).NotEmpty().WithMessage("Title in English is required").MaximumLength(500);
            RuleFor(x => x.TitleAr).NotEmpty().WithMessage("Title in Arabic is required").MaximumLength(500);
            RuleFor(x => x.DescriptionEn).MaximumLength(4000);
            RuleFor(x => x.DescriptionAr).MaximumLength(4000);
            RuleFor(x => x.IconName).MaximumLength(100);
            RuleFor(x => x.ImageUrl).MaximumLength(1024);
            RuleFor(x => x.LinkRoute).MaximumLength(500);
            RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0);
        }
    }
}

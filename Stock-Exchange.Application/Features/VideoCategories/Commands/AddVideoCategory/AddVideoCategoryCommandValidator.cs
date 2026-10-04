using FluentValidation;

namespace Stock_Exchange.Application.Features.VideoCategories.Commands.AddVideoCategory
{
    public class AddVideoCategoryCommandValidator : AbstractValidator<AddVideoCategoryCommand>
    {
        public AddVideoCategoryCommandValidator()
        {
            RuleFor(x => x.CategoryArName)
                .NotEmpty().WithMessage("Arabic category name is required.")
                .MaximumLength(150);

            RuleFor(x => x.CategoryEnName)
                .NotEmpty().WithMessage("English category name is required.")
                .MaximumLength(150);
        }
    }
}

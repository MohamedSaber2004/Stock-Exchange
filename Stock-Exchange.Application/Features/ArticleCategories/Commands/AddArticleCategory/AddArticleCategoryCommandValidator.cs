using FluentValidation;

namespace Stock_Exchange.Application.Features.ArticleCategories.Commands.AddArticleCategory
{
    public class AddArticleCategoryCommandValidator : AbstractValidator<AddArticleCategoryCommand>
    {
        public AddArticleCategoryCommandValidator()
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

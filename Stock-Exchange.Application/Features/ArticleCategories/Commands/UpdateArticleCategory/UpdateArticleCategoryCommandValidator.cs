using FluentValidation;

namespace Stock_Exchange.Application.Features.ArticleCategories.Commands.UpdateArticleCategory
{
    public class UpdateArticleCategoryCommandValidator : AbstractValidator<UpdateArticleCategoryCommand>
    {
        public UpdateArticleCategoryCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Category ID is required.");

            RuleFor(x => x.CategoryArName)
                .NotEmpty().WithMessage("Arabic category name is required.")
                .MaximumLength(150);

            RuleFor(x => x.CategoryEnName)
                .NotEmpty().WithMessage("English category name is required.")
                .MaximumLength(150);
        }
    }
}

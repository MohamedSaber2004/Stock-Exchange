using FluentValidation;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.ArticleCategories.Commands.AddArticleCategory
{
    public class AddArticleCategoryCommandValidator : AbstractValidator<AddArticleCategoryCommand>
    {
        public AddArticleCategoryCommandValidator()
        {
            RuleFor(x => x.CategoryArName)
                .NotEmpty().WithMessage(LocalizationKeys.ArticleCategoryMessages.CategoryArNameRequired)
                .MaximumLength(150).WithMessage(LocalizationKeys.ArticleCategoryMessages.CategoryArNameTooLong);

            RuleFor(x => x.CategoryEnName)
                .NotEmpty().WithMessage(LocalizationKeys.ArticleCategoryMessages.CategoryEnNameRequired)
                .MaximumLength(150).WithMessage(LocalizationKeys.ArticleCategoryMessages.CategoryEnNameTooLong);
        }
    }
}

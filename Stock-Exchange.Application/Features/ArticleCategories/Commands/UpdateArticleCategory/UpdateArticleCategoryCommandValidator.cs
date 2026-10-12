using FluentValidation;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.ArticleCategories.Commands.UpdateArticleCategory
{
    public class UpdateArticleCategoryCommandValidator : AbstractValidator<UpdateArticleCategoryCommand>
    {
        public UpdateArticleCategoryCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(LocalizationKeys.ArticleCategoryMessages.IdRequired);

            RuleFor(x => x.CategoryArName)
                .NotEmpty().WithMessage(LocalizationKeys.ArticleCategoryMessages.CategoryArNameRequired)
                .MaximumLength(150).WithMessage(LocalizationKeys.ArticleCategoryMessages.CategoryArNameTooLong);

            RuleFor(x => x.CategoryEnName)
                .NotEmpty().WithMessage(LocalizationKeys.ArticleCategoryMessages.CategoryEnNameRequired)
                .MaximumLength(150).WithMessage(LocalizationKeys.ArticleCategoryMessages.CategoryEnNameTooLong);
        }
    }
}

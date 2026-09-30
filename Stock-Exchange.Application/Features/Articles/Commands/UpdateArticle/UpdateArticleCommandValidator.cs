using FluentValidation;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.Articles.Commands.UpdateArticle
{
    public class UpdateArticleCommandValidator : AbstractValidator<UpdateArticleCommand>
    {
        public UpdateArticleCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(LocalizationKeys.ArticleMessages.IdRequired);

            RuleFor(x => x.TitleEn)
                .NotEmpty().WithMessage(LocalizationKeys.ArticleMessages.TitleEnRequired)
                .MaximumLength(500).WithMessage(LocalizationKeys.ArticleMessages.TitleEnTooLong);

            RuleFor(x => x.TitleAr)
                .NotEmpty().WithMessage(LocalizationKeys.ArticleMessages.TitleArRequired)
                .MaximumLength(500).WithMessage(LocalizationKeys.ArticleMessages.TitleArTooLong);

            RuleFor(x => x.ExcerptEn)
                .NotEmpty().WithMessage(LocalizationKeys.ArticleMessages.ExcerptEnRequired)
                .MaximumLength(2000).WithMessage(LocalizationKeys.ArticleMessages.ExcerptEnTooLong);

            RuleFor(x => x.ExcerptAr)
                .NotEmpty().WithMessage(LocalizationKeys.ArticleMessages.ExcerptArRequired)
                .MaximumLength(2000).WithMessage(LocalizationKeys.ArticleMessages.ExcerptArTooLong);

            RuleFor(x => x.AuthorName)
                .NotEmpty().WithMessage(LocalizationKeys.ArticleMessages.AuthorNameRequired)
                .MaximumLength(200).WithMessage(LocalizationKeys.ArticleMessages.AuthorNameTooLong);

            RuleFor(x => x.ImageUrl)
                .MaximumLength(1024).WithMessage(LocalizationKeys.ArticleMessages.ImageUrlTooLong)
                .When(x => !string.IsNullOrEmpty(x.ImageUrl));

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Display order must be 0 or greater.");
        }
    }
}

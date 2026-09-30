using FluentValidation;
using Stock_Exchange.Application.Features.Home.DTOs;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.Home.Commands.UpdateHomeArticles
{
    public class UpdateHomeArticlesCommandValidator : AbstractValidator<UpdateHomeArticlesCommand>
    {
        public UpdateHomeArticlesCommandValidator()
        {
            RuleForEach(x => x.Items).SetValidator(new HomeArticleItemRequestValidator());
        }
    }

    public class HomeArticleItemRequestValidator : AbstractValidator<HomeArticleItemRequest>
    {
        public HomeArticleItemRequestValidator()
        {
            RuleFor(x => x.TitleEn)
                .NotEmpty().WithMessage(LocalizationKeys.HomeMessages.HomeArticleTitleEnRequired)
                .MaximumLength(500).WithMessage(LocalizationKeys.HomeMessages.HomeArticleTitleEnTooLong);

            RuleFor(x => x.TitleAr)
                .NotEmpty().WithMessage(LocalizationKeys.HomeMessages.HomeArticleTitleArRequired)
                .MaximumLength(500).WithMessage(LocalizationKeys.HomeMessages.HomeArticleTitleArTooLong);

            RuleFor(x => x.ExcerptEn)
                .MaximumLength(2000).WithMessage(LocalizationKeys.HomeMessages.HomeArticleExcerptEnTooLong);

            RuleFor(x => x.ExcerptAr)
                .MaximumLength(2000).WithMessage(LocalizationKeys.HomeMessages.HomeArticleExcerptArTooLong);

            RuleFor(x => x.AuthorName)
                .MaximumLength(200).WithMessage(LocalizationKeys.HomeMessages.HomeArticleAuthorNameTooLong);

            RuleFor(x => x.ReadMinutes)
                .GreaterThanOrEqualTo(0).WithMessage(LocalizationKeys.HomeMessages.HomeArticleReadMinutesInvalid);

            RuleFor(x => x.ImageUrl)
                .MaximumLength(1024).WithMessage(LocalizationKeys.HomeMessages.HomeArticleImageUrlTooLong);
        }
    }
}

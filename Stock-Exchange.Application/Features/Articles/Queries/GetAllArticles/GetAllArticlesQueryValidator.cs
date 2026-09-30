using FluentValidation;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.Articles.Queries.GetAllArticles
{
    public class GetAllArticlesQueryValidator : AbstractValidator<GetAllArticlesQuery>
    {
        public GetAllArticlesQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .WithMessage(LocalizationKeys.ArticleMessages.PageNumberInvalid);

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .WithMessage(LocalizationKeys.ArticleMessages.PageSizeInvalid)
                .LessThanOrEqualTo(100)
                .WithMessage(LocalizationKeys.ArticleMessages.PageSizeTooLarge);

            RuleFor(x => x.Search)
                .MaximumLength(100)
                .WithMessage(LocalizationKeys.ArticleMessages.SearchTooLong)
                .When(x => !string.IsNullOrEmpty(x.Search));
        }
    }
}

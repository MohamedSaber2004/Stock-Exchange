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
                .WithMessage("Page number must be greater than 0.");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .WithMessage("Page size must be greater than 0.")
                .LessThanOrEqualTo(100)
                .WithMessage("Page size must not exceed 100.");

            RuleFor(x => x.Search)
                .MaximumLength(100)
                .WithMessage(LocalizationKeys.ArticleMessages.SearchTooLong)
                .When(x => !string.IsNullOrEmpty(x.Search));
        }
    }
}

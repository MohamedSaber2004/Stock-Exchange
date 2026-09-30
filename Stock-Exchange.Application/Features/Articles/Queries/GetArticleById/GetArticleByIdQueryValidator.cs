using FluentValidation;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.Articles.Queries.GetArticleById
{
    public class GetArticleByIdQueryValidator : AbstractValidator<GetArticleByIdQuery>
    {
        public GetArticleByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage(LocalizationKeys.ArticleMessages.IdRequired);
        }
    }
}

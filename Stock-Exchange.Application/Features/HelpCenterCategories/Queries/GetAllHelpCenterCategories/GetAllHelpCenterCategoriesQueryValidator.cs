using FluentValidation;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.HelpCenterCategories.Queries.GetAllHelpCenterCategories
{
    public class GetAllHelpCenterCategoriesQueryValidator : AbstractValidator<GetAllHelpCenterCategoriesQuery>
    {
        public GetAllHelpCenterCategoriesQueryValidator()
        {
            RuleFor(x => x.Search)
                .MaximumLength(200)
                .WithMessage(LocalizationKeys.HelpCenterMessages.SearchTooLong);
        }
    }
}

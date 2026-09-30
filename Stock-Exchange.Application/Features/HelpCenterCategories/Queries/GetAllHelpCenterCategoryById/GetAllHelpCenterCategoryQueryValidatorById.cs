using FluentValidation;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.HelpCenterCategories.Queries.GetAllHelpCenterCategoryById
{
    public class GetHelpCenterCategoryByIdQueryValidator : AbstractValidator<GetHelpCenterCategoryByIdQuery>
    {
        public GetHelpCenterCategoryByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage(LocalizationKeys.HelpCenterMessages.IdRequired);
        }
    }

    public class GetAllHelpCenterCategoryQueryValidatorById : AbstractValidator<GetAllHelpCenterCategoryQueryById>
    {
        public GetAllHelpCenterCategoryQueryValidatorById()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage(LocalizationKeys.HelpCenterMessages.IdRequired);
        }
    }
}

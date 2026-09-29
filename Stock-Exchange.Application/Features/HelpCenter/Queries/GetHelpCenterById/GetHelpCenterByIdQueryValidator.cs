using FluentValidation;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.HelpCenter.Queries.GetHelpCenterById
{
    public class GetHelpCenterByIdQueryValidator : AbstractValidator<GetHelpCenterByIdQuery>
    {
        public GetHelpCenterByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage(LocalizationKeys.HelpCenterMessages.IdRequired);
        }
    }
}

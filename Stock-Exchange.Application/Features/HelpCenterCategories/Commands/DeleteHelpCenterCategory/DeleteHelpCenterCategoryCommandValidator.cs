using FluentValidation;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.HelpCenterCategories.Commands.DeleteHelpCenterCategory
{
    public class DeleteHelpCenterCategoryCommandValidator : AbstractValidator<DeleteHelpCenterCategoryCommand>
    {
        public DeleteHelpCenterCategoryCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage(LocalizationKeys.HelpCenterMessages.IdRequired);
        }
    }
}

using FluentValidation;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.HelpCenter.Commands.DeleteHelpCenter
{
    public class DeleteHelpCenterCommandValidator : AbstractValidator<DeleteHelpCenterCommand>
    {
        public DeleteHelpCenterCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage(LocalizationKeys.HelpCenterMessages.IdRequired);
        }
    }
}

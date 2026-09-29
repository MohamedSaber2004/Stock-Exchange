using FluentValidation;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.HelpCenter.Commands.UpdateHelpCenter
{
    public class UpdateHelpCenterCommandValidator : AbstractValidator<UpdateHelpCenterCommand>
    {
        public UpdateHelpCenterCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage(LocalizationKeys.HelpCenterMessages.IdRequired);

            RuleFor(x => x)
                .Must(x => !string.IsNullOrWhiteSpace(x.TitleEn) || !string.IsNullOrWhiteSpace(x.TitleAr))
                .WithMessage(LocalizationKeys.HelpCenterMessages.TitleRequired);

            RuleFor(x => x)
                .Must(x => !string.IsNullOrWhiteSpace(x.ContentEn) || !string.IsNullOrWhiteSpace(x.ContentAr))
                .WithMessage(LocalizationKeys.HelpCenterMessages.ContentRequired);

            RuleFor(x => x.TitleEn).MaximumLength(200).WithMessage(LocalizationKeys.HelpCenterMessages.TitleTooLong);
            RuleFor(x => x.TitleAr).MaximumLength(200).WithMessage(LocalizationKeys.HelpCenterMessages.TitleTooLong);
            RuleFor(x => x.ContentEn).MaximumLength(4000).WithMessage(LocalizationKeys.HelpCenterMessages.ContentTooLong);
            RuleFor(x => x.ContentAr).MaximumLength(4000).WithMessage(LocalizationKeys.HelpCenterMessages.ContentTooLong);
        }
    }
}

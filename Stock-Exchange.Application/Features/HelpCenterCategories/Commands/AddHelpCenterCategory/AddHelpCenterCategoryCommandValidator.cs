using FluentValidation;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.HelpCenterCategories.Commands.AddHelpCenterCategory
{
    public class AddHelpCenterCategoryCommandValidator : AbstractValidator<AddHelpCenterCategoryCommand>
    {
        public AddHelpCenterCategoryCommandValidator()
        {
            RuleFor(x => x)
                .Must(x => !string.IsNullOrWhiteSpace(x.TitleEn) || !string.IsNullOrWhiteSpace(x.TitleAr))
                .WithMessage(LocalizationKeys.HelpCenterMessages.TitleRequired);

            RuleFor(x => x.TitleEn)
                .MaximumLength(150)
                .WithMessage(LocalizationKeys.HelpCenterMessages.TitleTooLong);

            RuleFor(x => x.TitleAr)
                .MaximumLength(150)
                .WithMessage(LocalizationKeys.HelpCenterMessages.TitleTooLong);
        }
    }
}

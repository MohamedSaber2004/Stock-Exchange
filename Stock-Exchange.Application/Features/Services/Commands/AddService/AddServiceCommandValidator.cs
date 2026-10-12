using FluentValidation;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.Services.Commands.AddService
{
    public class AddServiceCommandValidator : AbstractValidator<AddServiceCommand>
    {
        public AddServiceCommandValidator()
        {
            RuleFor(x => x.TitleEn)
                .NotEmpty().WithMessage(LocalizationKeys.ServiceMessages.TitleEnRequired)
                .MaximumLength(500).WithMessage(LocalizationKeys.ServiceMessages.TitleEnTooLong);

            RuleFor(x => x.TitleAr)
                .NotEmpty().WithMessage(LocalizationKeys.ServiceMessages.TitleArRequired)
                .MaximumLength(500).WithMessage(LocalizationKeys.ServiceMessages.TitleArTooLong);

            RuleFor(x => x.DescriptionEn)
                .MaximumLength(4000).WithMessage(LocalizationKeys.ServiceMessages.DescriptionEnTooLong)
                .When(x => !string.IsNullOrEmpty(x.DescriptionEn));

            RuleFor(x => x.DescriptionAr)
                .MaximumLength(4000).WithMessage(LocalizationKeys.ServiceMessages.DescriptionArTooLong)
                .When(x => !string.IsNullOrEmpty(x.DescriptionAr));

            RuleFor(x => x.IconName)
                .MaximumLength(100).WithMessage(LocalizationKeys.ServiceMessages.IconNameTooLong)
                .When(x => !string.IsNullOrEmpty(x.IconName));

            RuleFor(x => x.ImageUrl)
                .MaximumLength(1024).WithMessage(LocalizationKeys.ServiceMessages.ImageUrlTooLong)
                .When(x => !string.IsNullOrEmpty(x.ImageUrl));

            RuleFor(x => x.LinkRoute)
                .MaximumLength(500).WithMessage(LocalizationKeys.ServiceMessages.LinkRouteTooLong)
                .When(x => !string.IsNullOrEmpty(x.LinkRoute));

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage(LocalizationKeys.ServiceMessages.DisplayOrderInvalid);
        }
    }
}

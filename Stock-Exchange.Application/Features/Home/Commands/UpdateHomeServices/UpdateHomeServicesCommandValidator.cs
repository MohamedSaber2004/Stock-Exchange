using FluentValidation;
using Stock_Exchange.Application.Features.Home.DTOs;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.Home.Commands.UpdateHomeServices
{
    public class UpdateHomeServicesCommandValidator : AbstractValidator<UpdateHomeServicesCommand>
    {
        public UpdateHomeServicesCommandValidator()
        {
            RuleForEach(x => x.Items).SetValidator(new HomeServiceItemRequestValidator());
        }
    }

    public class HomeServiceItemRequestValidator : AbstractValidator<HomeServiceItemRequest>
    {
        public HomeServiceItemRequestValidator()
        {
            RuleFor(x => x.TitleEn)
                .NotEmpty().WithMessage(LocalizationKeys.HomeMessages.HomeServiceTitleEnRequired)
                .MaximumLength(200).WithMessage(LocalizationKeys.HomeMessages.HomeServiceTitleEnTooLong);

            RuleFor(x => x.TitleAr)
                .NotEmpty().WithMessage(LocalizationKeys.HomeMessages.HomeServiceTitleArRequired)
                .MaximumLength(200).WithMessage(LocalizationKeys.HomeMessages.HomeServiceTitleArTooLong);

            RuleFor(x => x.DescriptionEn)
                .MaximumLength(1000).WithMessage(LocalizationKeys.HomeMessages.HomeServiceDescriptionEnTooLong);

            RuleFor(x => x.DescriptionAr)
                .MaximumLength(1000).WithMessage(LocalizationKeys.HomeMessages.HomeServiceDescriptionArTooLong);

            RuleFor(x => x.IconName)
                .MaximumLength(100).WithMessage(LocalizationKeys.HomeMessages.HomeServiceIconNameTooLong);

            RuleFor(x => x.ImageUrl)
                .MaximumLength(1024).WithMessage(LocalizationKeys.HomeMessages.HomeServiceImageUrlTooLong);

            RuleFor(x => x.LinkRoute)
                .MaximumLength(500).WithMessage(LocalizationKeys.HomeMessages.HomeServiceLinkRouteTooLong);
        }
    }
}

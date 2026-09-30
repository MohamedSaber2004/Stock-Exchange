using FluentValidation;
using Stock_Exchange.Application.Features.Home.DTOs;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.Home.Commands.UpdateHomeExperts
{
    public class UpdateHomeExpertsCommandValidator : AbstractValidator<UpdateHomeExpertsCommand>
    {
        public UpdateHomeExpertsCommandValidator()
        {
            RuleForEach(x => x.Items).SetValidator(new HomeExpertItemRequestValidator());
        }
    }

    public class HomeExpertItemRequestValidator : AbstractValidator<HomeExpertItemRequest>
    {
        public HomeExpertItemRequestValidator()
        {
            RuleFor(x => x.FullNameEn)
                .NotEmpty().WithMessage(LocalizationKeys.HomeMessages.HomeExpertFullNameEnRequired)
                .MaximumLength(200).WithMessage(LocalizationKeys.HomeMessages.HomeExpertFullNameEnTooLong);

            RuleFor(x => x.FullNameAr)
                .NotEmpty().WithMessage(LocalizationKeys.HomeMessages.HomeExpertFullNameArRequired)
                .MaximumLength(200).WithMessage(LocalizationKeys.HomeMessages.HomeExpertFullNameArTooLong);

            RuleFor(x => x.TitleEn)
                .NotEmpty().WithMessage(LocalizationKeys.HomeMessages.HomeExpertTitleEnRequired)
                .MaximumLength(200).WithMessage(LocalizationKeys.HomeMessages.HomeExpertTitleEnTooLong);

            RuleFor(x => x.TitleAr)
                .NotEmpty().WithMessage(LocalizationKeys.HomeMessages.HomeExpertTitleArRequired)
                .MaximumLength(200).WithMessage(LocalizationKeys.HomeMessages.HomeExpertTitleArTooLong);

            RuleFor(x => x.AvatarUrl)
                .MaximumLength(1024).WithMessage(LocalizationKeys.HomeMessages.HomeExpertAvatarUrlTooLong);
        }
    }
}

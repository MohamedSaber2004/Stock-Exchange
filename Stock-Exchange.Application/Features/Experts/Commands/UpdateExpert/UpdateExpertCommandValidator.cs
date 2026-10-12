using FluentValidation;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.Experts.Commands.UpdateExpert
{
    public class UpdateExpertCommandValidator : AbstractValidator<UpdateExpertCommand>
    {
        public UpdateExpertCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(LocalizationKeys.ExpertMessages.IdRequired);

            RuleFor(x => x.FullNameEn)
                .NotEmpty().WithMessage(LocalizationKeys.ExpertMessages.FullNameEnRequired)
                .MaximumLength(200).WithMessage(LocalizationKeys.ExpertMessages.FullNameEnTooLong);

            RuleFor(x => x.FullNameAr)
                .NotEmpty().WithMessage(LocalizationKeys.ExpertMessages.FullNameArRequired)
                .MaximumLength(200).WithMessage(LocalizationKeys.ExpertMessages.FullNameArTooLong);

            RuleFor(x => x.TitleEn)
                .NotEmpty().WithMessage(LocalizationKeys.ExpertMessages.TitleEnRequired)
                .MaximumLength(200).WithMessage(LocalizationKeys.ExpertMessages.TitleEnTooLong);

            RuleFor(x => x.TitleAr)
                .NotEmpty().WithMessage(LocalizationKeys.ExpertMessages.TitleArRequired)
                .MaximumLength(200).WithMessage(LocalizationKeys.ExpertMessages.TitleArTooLong);

            RuleFor(x => x.AvatarUrl)
                .MaximumLength(1024).WithMessage(LocalizationKeys.ExpertMessages.AvatarUrlTooLong)
                .When(x => !string.IsNullOrEmpty(x.AvatarUrl));

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage(LocalizationKeys.ExpertMessages.DisplayOrderInvalid);
        }
    }
}

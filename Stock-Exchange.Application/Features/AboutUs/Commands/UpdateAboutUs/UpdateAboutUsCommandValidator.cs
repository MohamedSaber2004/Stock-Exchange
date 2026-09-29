using FluentValidation;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.AboutUs.Commands.UpdateAboutUs
{
    public class UpdateAboutUsCommandValidator : AbstractValidator<UpdateAboutUsCommand>
    {
        public UpdateAboutUsCommandValidator()
        {
            RuleFor(x => x)
                .Must(x => !string.IsNullOrWhiteSpace(x.StoryEn) || !string.IsNullOrWhiteSpace(x.StoryAr))
                .WithMessage(LocalizationKeys.AboutUsMessages.StoryRequired);

            RuleFor(x => x)
                .Must(x => !string.IsNullOrWhiteSpace(x.MissionEn) || !string.IsNullOrWhiteSpace(x.MissionAr))
                .WithMessage(LocalizationKeys.AboutUsMessages.MissionRequired);

            RuleFor(x => x)
                .Must(x => !string.IsNullOrWhiteSpace(x.VisionEn) || !string.IsNullOrWhiteSpace(x.VisionAr))
                .WithMessage(LocalizationKeys.AboutUsMessages.VisionRequired);

            RuleFor(x => x.StoryEn).MaximumLength(4000).WithMessage(LocalizationKeys.AboutUsMessages.StoryTooLong);
            RuleFor(x => x.StoryAr).MaximumLength(4000).WithMessage(LocalizationKeys.AboutUsMessages.StoryTooLong);
            RuleFor(x => x.MissionEn).MaximumLength(4000).WithMessage(LocalizationKeys.AboutUsMessages.MissionTooLong);
            RuleFor(x => x.MissionAr).MaximumLength(4000).WithMessage(LocalizationKeys.AboutUsMessages.MissionTooLong);
            RuleFor(x => x.VisionEn).MaximumLength(4000).WithMessage(LocalizationKeys.AboutUsMessages.VisionTooLong);
            RuleFor(x => x.VisionAr).MaximumLength(4000).WithMessage(LocalizationKeys.AboutUsMessages.VisionTooLong);

            RuleForEach(x => x.Features)
                .Must(f => !string.IsNullOrWhiteSpace(f.TitleEn) || !string.IsNullOrWhiteSpace(f.TitleAr))
                .WithMessage(LocalizationKeys.AboutUsMessages.FeatureTitleRequired);

            RuleForEach(x => x.Features)
                .Must(f => !string.IsNullOrWhiteSpace(f.DescriptionEn) || !string.IsNullOrWhiteSpace(f.DescriptionAr))
                .WithMessage(LocalizationKeys.AboutUsMessages.FeatureDescriptionRequired);

            RuleForEach(x => x.Features)
                .Must(f => (f.TitleEn?.Trim().Length ?? 0) <= 200 && (f.TitleAr?.Trim().Length ?? 0) <= 200)
                .WithMessage(LocalizationKeys.AboutUsMessages.FeatureTitleTooLong);

            RuleForEach(x => x.Features)
                .Must(f => (f.DescriptionEn?.Trim().Length ?? 0) <= 500 && (f.DescriptionAr?.Trim().Length ?? 0) <= 500)
                .WithMessage(LocalizationKeys.AboutUsMessages.FeatureDescriptionTooLong);

            RuleForEach(x => x.Features)
                .Must(f => (f.Category?.Trim().Length ?? 0) <= 50)
                .WithMessage(LocalizationKeys.AboutUsMessages.FeatureCategoryTooLong);
        }
    }
}

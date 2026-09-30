using FluentValidation;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.AboutUs.Commands.UpdateAboutUs
{
    public class UpdateAboutUsCommandValidator : AbstractValidator<UpdateAboutUsCommand>
    {
        public UpdateAboutUsCommandValidator()
        {
            RuleFor(x => x.StoryEn)
                .MaximumLength(4000)
                .When(x => x.StoryEn != null)
                .WithMessage(LocalizationKeys.AboutUsMessages.StoryTooLong);

            RuleFor(x => x.StoryAr)
                .MaximumLength(4000)
                .When(x => x.StoryAr != null)
                .WithMessage(LocalizationKeys.AboutUsMessages.StoryTooLong);

            RuleFor(x => x.MissionEn)
                .MaximumLength(4000)
                .When(x => x.MissionEn != null)
                .WithMessage(LocalizationKeys.AboutUsMessages.MissionTooLong);

            RuleFor(x => x.MissionAr)
                .MaximumLength(4000)
                .When(x => x.MissionAr != null)
                .WithMessage(LocalizationKeys.AboutUsMessages.MissionTooLong);

            RuleFor(x => x.VisionEn)
                .MaximumLength(4000)
                .When(x => x.VisionEn != null)
                .WithMessage(LocalizationKeys.AboutUsMessages.VisionTooLong);

            RuleFor(x => x.VisionAr)
                .MaximumLength(4000)
                .When(x => x.VisionAr != null)
                .WithMessage(LocalizationKeys.AboutUsMessages.VisionTooLong);

            RuleFor(x => x.SupportEmail)
                .EmailAddress()
                .WithMessage(LocalizationKeys.AuthMessages.InvalidEmail)
                .MaximumLength(256)
                .WithMessage(LocalizationKeys.AuthMessages.InvalidEmail)
                .When(x => !string.IsNullOrWhiteSpace(x.SupportEmail));

            When(x => x.Features != null, () =>
            {
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
            });
        }
    }
}

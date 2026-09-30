using FluentValidation;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.PrivacyPolicy.Commands.UpdatePrivacy
{
    public class UpdatePrivacyCommandValidator : AbstractValidator<UpdatePrivacyCommand>
    {
        public UpdatePrivacyCommandValidator()
        {
            RuleFor(x => x.TitleEn)
                .MaximumLength(200)
                .When(x => x.TitleEn != null)
                .WithMessage(LocalizationKeys.PrivacyPolicyMessages.TitleTooLong);

            RuleFor(x => x.TitleAr)
                .MaximumLength(200)
                .When(x => x.TitleAr != null)
                .WithMessage(LocalizationKeys.PrivacyPolicyMessages.TitleTooLong);

            RuleFor(x => x.DescriptionEn)
                .MaximumLength(4000)
                .When(x => x.DescriptionEn != null)
                .WithMessage(LocalizationKeys.PrivacyPolicyMessages.DescriptionTooLong);

            RuleFor(x => x.DescriptionAr)
                .MaximumLength(4000)
                .When(x => x.DescriptionAr != null)
                .WithMessage(LocalizationKeys.PrivacyPolicyMessages.DescriptionTooLong);

            When(x => x.Sections != null, () =>
            {
                RuleForEach(x => x.Sections)
                    .Must(s => !string.IsNullOrWhiteSpace(s.TitleEn) || !string.IsNullOrWhiteSpace(s.TitleAr))
                    .WithMessage(LocalizationKeys.PrivacyPolicyMessages.SectionTitleRequired);

                RuleForEach(x => x.Sections)
                    .Must(s => !string.IsNullOrWhiteSpace(s.ContentEn) || !string.IsNullOrWhiteSpace(s.ContentAr))
                    .WithMessage(LocalizationKeys.PrivacyPolicyMessages.SectionContentRequired);

                RuleForEach(x => x.Sections)
                    .Must(s => (s.TitleEn?.Trim().Length ?? 0) <= 250 && (s.TitleAr?.Trim().Length ?? 0) <= 250)
                    .WithMessage(LocalizationKeys.PrivacyPolicyMessages.SectionTitleTooLong);

                RuleForEach(x => x.Sections)
                    .Must(s => (s.ContentEn?.Trim().Length ?? 0) <= 8000 && (s.ContentAr?.Trim().Length ?? 0) <= 8000)
                    .WithMessage(LocalizationKeys.PrivacyPolicyMessages.SectionContentTooLong);
            });
        }
    }
}

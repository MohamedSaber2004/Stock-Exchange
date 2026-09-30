using FluentValidation;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.Videos.Commands.AddVideo
{
    public class AddVideoCommandValidator : AbstractValidator<AddVideoCommand>
    {
        public AddVideoCommandValidator()
        {
            RuleFor(x => x.TitleEn)
                .NotEmpty().WithMessage(LocalizationKeys.VideoMessages.TitleEnRequired)
                .MaximumLength(500).WithMessage(LocalizationKeys.VideoMessages.TitleEnTooLong);

            RuleFor(x => x.TitleAr)
                .NotEmpty().WithMessage(LocalizationKeys.VideoMessages.TitleArRequired)
                .MaximumLength(500).WithMessage(LocalizationKeys.VideoMessages.TitleArTooLong);

            RuleFor(x => x.InstructorName)
                .NotEmpty().WithMessage(LocalizationKeys.VideoMessages.InstructorNameRequired)
                .MaximumLength(200).WithMessage(LocalizationKeys.VideoMessages.InstructorNameTooLong);

            RuleFor(x => x.CategoryEn)
                .NotEmpty().WithMessage(LocalizationKeys.VideoMessages.CategoryEnRequired)
                .MaximumLength(100).WithMessage(LocalizationKeys.VideoMessages.CategoryEnTooLong);

            RuleFor(x => x.CategoryAr)
                .NotEmpty().WithMessage(LocalizationKeys.VideoMessages.CategoryArRequired)
                .MaximumLength(100).WithMessage(LocalizationKeys.VideoMessages.CategoryArTooLong);

            RuleFor(x => x.ThumbnailUrl)
                .MaximumLength(1024).WithMessage(LocalizationKeys.VideoMessages.ThumbnailUrlTooLong)
                .When(x => !string.IsNullOrEmpty(x.ThumbnailUrl));

            RuleFor(x => x.VideoUrl)
                .MaximumLength(1024).WithMessage(LocalizationKeys.VideoMessages.VideoUrlTooLong)
                .When(x => !string.IsNullOrEmpty(x.VideoUrl));

            RuleFor(x => x.DurationSeconds)
                .GreaterThanOrEqualTo(0).WithMessage(LocalizationKeys.VideoMessages.DurationSecondsInvalid);

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage(LocalizationKeys.VideoMessages.DisplayOrderInvalid);
        }
    }
}

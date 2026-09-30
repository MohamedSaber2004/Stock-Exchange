using FluentValidation;
using Stock_Exchange.Application.Features.Home.DTOs;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.Home.Commands.UpdateHomeVideos
{
    public class UpdateHomeVideosCommandValidator : AbstractValidator<UpdateHomeVideosCommand>
    {
        public UpdateHomeVideosCommandValidator()
        {
            RuleForEach(x => x.Items).SetValidator(new HomeVideoItemRequestValidator());
        }
    }

    public class HomeVideoItemRequestValidator : AbstractValidator<HomeVideoItemRequest>
    {
        public HomeVideoItemRequestValidator()
        {
            RuleFor(x => x.TitleEn)
                .NotEmpty().WithMessage(LocalizationKeys.HomeMessages.HomeVideoTitleEnRequired)
                .MaximumLength(500).WithMessage(LocalizationKeys.HomeMessages.HomeVideoTitleEnTooLong);

            RuleFor(x => x.TitleAr)
                .NotEmpty().WithMessage(LocalizationKeys.HomeMessages.HomeVideoTitleArRequired)
                .MaximumLength(500).WithMessage(LocalizationKeys.HomeMessages.HomeVideoTitleArTooLong);

            RuleFor(x => x.InstructorName)
                .MaximumLength(200).WithMessage(LocalizationKeys.HomeMessages.HomeVideoInstructorNameTooLong);

            RuleFor(x => x.CategoryEn)
                .MaximumLength(200).WithMessage(LocalizationKeys.HomeMessages.HomeVideoCategoryEnTooLong);

            RuleFor(x => x.CategoryAr)
                .MaximumLength(200).WithMessage(LocalizationKeys.HomeMessages.HomeVideoCategoryArTooLong);

            RuleFor(x => x.ThumbnailUrl)
                .MaximumLength(1024).WithMessage(LocalizationKeys.HomeMessages.HomeVideoThumbnailUrlTooLong);

            RuleFor(x => x.VideoUrl)
                .MaximumLength(1024).WithMessage(LocalizationKeys.HomeMessages.HomeVideoUrlTooLong);
        }
    }
}

using FluentValidation;
using Stock_Exchange.Application.Features.Home.DTOs;

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
                .NotEmpty().WithMessage("Video English title is required.")
                .MaximumLength(500).WithMessage("Video English title must not exceed 500 characters.");

            RuleFor(x => x.TitleAr)
                .NotEmpty().WithMessage("Video Arabic title is required.")
                .MaximumLength(500).WithMessage("Video Arabic title must not exceed 500 characters.");

            RuleFor(x => x.InstructorName)
                .MaximumLength(200).WithMessage("Instructor name must not exceed 200 characters.");

            RuleFor(x => x.CategoryEn)
                .MaximumLength(200).WithMessage("Video English category must not exceed 200 characters.");

            RuleFor(x => x.CategoryAr)
                .MaximumLength(200).WithMessage("Video Arabic category must not exceed 200 characters.");

            RuleFor(x => x.ThumbnailUrl)
                .MaximumLength(1024).WithMessage("Thumbnail URL must not exceed 1024 characters.");

            RuleFor(x => x.VideoUrl)
                .MaximumLength(1024).WithMessage("Video URL must not exceed 1024 characters.");
        }
    }
}

using FluentValidation;
using Stock_Exchange.Application.Features.Home.DTOs;

namespace Stock_Exchange.Application.Features.Home.Commands.UpdateHomeNews
{
    public class UpdateHomeNewsCommandValidator : AbstractValidator<UpdateHomeNewsCommand>
    {
        public UpdateHomeNewsCommandValidator()
        {
            RuleForEach(x => x.Items).SetValidator(new HomeNewsItemRequestValidator());
        }
    }

    public class HomeNewsItemRequestValidator : AbstractValidator<HomeNewsItemRequest>
    {
        public HomeNewsItemRequestValidator()
        {
            RuleFor(x => x.TitleEn)
                .NotEmpty().WithMessage("News English title is required.")
                .MaximumLength(500).WithMessage("News English title must not exceed 500 characters.");

            RuleFor(x => x.TitleAr)
                .NotEmpty().WithMessage("News Arabic title is required.")
                .MaximumLength(500).WithMessage("News Arabic title must not exceed 500 characters.");

            RuleFor(x => x.SummaryEn)
                .MaximumLength(4000).WithMessage("News English summary must not exceed 4000 characters.");

            RuleFor(x => x.SummaryAr)
                .MaximumLength(4000).WithMessage("News Arabic summary must not exceed 4000 characters.");

            RuleFor(x => x.CategoryEn)
                .MaximumLength(200).WithMessage("News English category must not exceed 200 characters.");

            RuleFor(x => x.CategoryAr)
                .MaximumLength(200).WithMessage("News Arabic category must not exceed 200 characters.");

            RuleFor(x => x.ImageUrl)
                .MaximumLength(1024).WithMessage("Image URL must not exceed 1024 characters.");
        }
    }
}

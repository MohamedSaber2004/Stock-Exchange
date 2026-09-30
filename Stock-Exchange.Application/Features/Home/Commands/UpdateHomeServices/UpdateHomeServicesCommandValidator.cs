using FluentValidation;
using Stock_Exchange.Application.Features.Home.DTOs;

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
                .NotEmpty().WithMessage("Service English title is required.")
                .MaximumLength(200).WithMessage("Service English title must not exceed 200 characters.");

            RuleFor(x => x.TitleAr)
                .NotEmpty().WithMessage("Service Arabic title is required.")
                .MaximumLength(200).WithMessage("Service Arabic title must not exceed 200 characters.");

            RuleFor(x => x.DescriptionEn)
                .MaximumLength(1000).WithMessage("Service English description must not exceed 1000 characters.");

            RuleFor(x => x.DescriptionAr)
                .MaximumLength(1000).WithMessage("Service Arabic description must not exceed 1000 characters.");

            RuleFor(x => x.IconName)
                .MaximumLength(100).WithMessage("Icon name must not exceed 100 characters.");

            RuleFor(x => x.ImageUrl)
                .MaximumLength(1024).WithMessage("Image URL must not exceed 1024 characters.");

            RuleFor(x => x.LinkRoute)
                .MaximumLength(500).WithMessage("Link route must not exceed 500 characters.");
        }
    }
}

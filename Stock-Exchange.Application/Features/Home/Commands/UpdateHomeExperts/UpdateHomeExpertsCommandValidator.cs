using FluentValidation;
using Stock_Exchange.Application.Features.Home.DTOs;

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
                .NotEmpty().WithMessage("Expert English full name is required.")
                .MaximumLength(200).WithMessage("Expert English full name must not exceed 200 characters.");

            RuleFor(x => x.FullNameAr)
                .NotEmpty().WithMessage("Expert Arabic full name is required.")
                .MaximumLength(200).WithMessage("Expert Arabic full name must not exceed 200 characters.");

            RuleFor(x => x.TitleEn)
                .NotEmpty().WithMessage("Expert English title/specialty is required.")
                .MaximumLength(200).WithMessage("Expert English title must not exceed 200 characters.");

            RuleFor(x => x.TitleAr)
                .NotEmpty().WithMessage("Expert Arabic title/specialty is required.")
                .MaximumLength(200).WithMessage("Expert Arabic title must not exceed 200 characters.");

            RuleFor(x => x.AvatarUrl)
                .MaximumLength(1024).WithMessage("Avatar URL must not exceed 1024 characters.");
        }
    }
}

using FluentValidation;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.VideoCategories.Commands.AddVideoCategory
{
    public class AddVideoCategoryCommandValidator : AbstractValidator<AddVideoCategoryCommand>
    {
        public AddVideoCategoryCommandValidator()
        {
            RuleFor(x => x.CategoryArName)
                .NotEmpty().WithMessage(LocalizationKeys.VideoCategoryMessages.CategoryArNameRequired)
                .MaximumLength(150).WithMessage(LocalizationKeys.VideoCategoryMessages.CategoryArNameTooLong);

            RuleFor(x => x.CategoryEnName)
                .NotEmpty().WithMessage(LocalizationKeys.VideoCategoryMessages.CategoryEnNameRequired)
                .MaximumLength(150).WithMessage(LocalizationKeys.VideoCategoryMessages.CategoryEnNameTooLong);
        }
    }
}

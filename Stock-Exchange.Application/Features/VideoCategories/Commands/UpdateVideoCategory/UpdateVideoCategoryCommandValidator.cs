using FluentValidation;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.VideoCategories.Commands.UpdateVideoCategory
{
    public class UpdateVideoCategoryCommandValidator : AbstractValidator<UpdateVideoCategoryCommand>
    {
        public UpdateVideoCategoryCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(LocalizationKeys.VideoCategoryMessages.IdRequired);

            RuleFor(x => x.CategoryArName)
                .NotEmpty().WithMessage(LocalizationKeys.VideoCategoryMessages.CategoryArNameRequired)
                .MaximumLength(150).WithMessage(LocalizationKeys.VideoCategoryMessages.CategoryArNameTooLong);

            RuleFor(x => x.CategoryEnName)
                .NotEmpty().WithMessage(LocalizationKeys.VideoCategoryMessages.CategoryEnNameRequired)
                .MaximumLength(150).WithMessage(LocalizationKeys.VideoCategoryMessages.CategoryEnNameTooLong);
        }
    }
}

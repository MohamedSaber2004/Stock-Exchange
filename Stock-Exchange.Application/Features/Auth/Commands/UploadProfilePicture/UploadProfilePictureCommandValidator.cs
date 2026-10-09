using FluentValidation;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Services;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.Auth.Commands.UploadProfilePicture
{
    public class UploadProfilePictureCommandValidator : AbstractValidator<UploadProfilePictureCommand>
    {
        public UploadProfilePictureCommandValidator(IImageValidator imageValidator)
        {
            RuleFor(x => x.File)
                .NotNull()
                .WithMessage(LocalizationKeys.Attachments.FileEmpty);

            RuleFor(x => x.Place)
                .GreaterThanOrEqualTo(0)
                .Must(place => !string.IsNullOrWhiteSpace(UploadPaths.GetPath(place)))
                .WithMessage(LocalizationKeys.ExceptionMessages.BadRequest);

            RuleFor(x => x.MediaType)
                .Equal(MediaType.Image)
                .WithMessage(LocalizationKeys.Attachments.InvalidImageFormat);

            RuleFor(x => x.File)
                .Must(file => imageValidator.IsValidImage(file))
                .WithMessage(LocalizationKeys.Attachments.InvalidImageFormat)
                .When(x => x.File != null);
        }
    }
}

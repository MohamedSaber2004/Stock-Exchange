using FluentValidation;
using Microsoft.AspNetCore.Http;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.Attachments.Commands.UploadFile
{
    public class UploadFileCommandValidator : AbstractValidator<UploadFileCommand>
    {
        private readonly IImageValidator _imageValidator;
        private readonly IVideoValidator _videoValidator;
        private readonly IAudioValidator _audioValidator;
        private readonly IFileValidator _fileValidator;

        public UploadFileCommandValidator(
            IImageValidator imageValidator,
            IVideoValidator videoValidator,
            IAudioValidator audioValidator,
            IFileValidator fileValidator)
        {
            _imageValidator = imageValidator;
            _videoValidator = videoValidator;
            _audioValidator = audioValidator;
            _fileValidator = fileValidator;

            RuleFor(x => x.File)
                .NotNull()
                .WithMessage(LocalizationKeys.Attachments.FileEmpty);

            RuleFor(x => x.Place)
                .GreaterThanOrEqualTo(0)
                .WithMessage(LocalizationKeys.ExceptionMessages.BadRequest);

            RuleFor(x => x)
                .Must(IsValidForMediaType)
                .WithMessage(GetInvalidFormatKey)
                .When(x => x.File != null);
        }

        private bool IsValidForMediaType(UploadFileCommand command)
        {
            return IsValidForMediaType(command.MediaType, command.File);
        }

        private bool IsValidForMediaType(MediaType mediaType, IFormFile? file)
        {
            return mediaType switch
            {
                MediaType.Image => _imageValidator.IsValidImage(file),
                MediaType.Video => _videoValidator.IsValidVideo(file),
                MediaType.Audio => _audioValidator.IsValidAudio(file),
                MediaType.File => _fileValidator.IsValidFile(file),
                _ => false
            };
        }

        private static string GetInvalidFormatKey(UploadFileCommand command)
        {
            return command.MediaType switch
            {
                MediaType.Image => LocalizationKeys.Attachments.InvalidImageFormat,
                MediaType.Video => LocalizationKeys.Attachments.InvalidVideoFormat,
                MediaType.Audio => LocalizationKeys.Attachments.InvalidAudioFormat,
                MediaType.File => LocalizationKeys.Attachments.InvalidFileFormat,
                _ => LocalizationKeys.Attachments.InvalidFormat
            };
        }
    }
}

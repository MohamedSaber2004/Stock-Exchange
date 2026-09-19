using FluentValidation;
using Microsoft.AspNetCore.Http;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.Attachments.Commands.UploadMultipleFiles
{
    public class UploadMultipleFilesCommandValidator : AbstractValidator<UploadMultipleFilesCommand>
    {
        private readonly IImageValidator _imageValidator;
        private readonly IVideoValidator _videoValidator;
        private readonly IAudioValidator _audioValidator;
        private readonly IFileValidator _fileValidator;

        public UploadMultipleFilesCommandValidator(
            IImageValidator imageValidator,
            IVideoValidator videoValidator,
            IAudioValidator audioValidator,
            IFileValidator fileValidator)
        {
            _imageValidator = imageValidator;
            _videoValidator = videoValidator;
            _audioValidator = audioValidator;
            _fileValidator = fileValidator;

            RuleFor(x => x.Files)
                .NotNull()
                .NotEmpty()
                .WithMessage(LocalizationKeys.Attachments.NoMediaProvided);

            RuleFor(x => x.Place)
                .GreaterThanOrEqualTo(0)
                .WithMessage(LocalizationKeys.ExceptionMessages.BadRequest);

            RuleForEach(x => x.Files)
                .Must((command, file) => IsValidForMediaType(command.MediaType, file))
                .WithMessage(command => GetInvalidFormatKey(command.MediaType))
                .When(x => x.Files != null);
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

        private static string GetInvalidFormatKey(MediaType mediaType)
        {
            return mediaType switch
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

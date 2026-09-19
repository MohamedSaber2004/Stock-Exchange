using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.Attachments.Commands.UpdateFile
{
    public class UpdateFileCommandHandler : IRequestHandler<UpdateFileCommand, Result<string>>
    {
        private readonly IImageValidator _imageValidator;
        private readonly IVideoValidator _videoValidator;
        private readonly IAudioValidator _audioValidator;
        private readonly IFileValidator _fileValidator;
        private readonly IStringLocalizer<Messages> _localizer;

        public UpdateFileCommandHandler(
            IImageValidator imageValidator,
            IVideoValidator videoValidator,
            IAudioValidator audioValidator,
            IFileValidator fileValidator,
            IStringLocalizer<Messages> localizer)
        {
            _imageValidator = imageValidator;
            _videoValidator = videoValidator;
            _audioValidator = audioValidator;
            _fileValidator = fileValidator;
            _localizer = localizer;
        }

        public async Task<Result<string>> Handle(UpdateFileCommand request, CancellationToken cancellationToken)
        {
            var (uploaded, result) = request.MediaType switch
            {
                MediaType.Image => await _imageValidator.UploadImage(request.File, request.Place),
                MediaType.Video => await _videoValidator.UploadVideo(request.File, request.Place),
                MediaType.Audio => await _audioValidator.UploadAudio(request.File, request.Place),
                MediaType.File => await _fileValidator.UploadFile(request.File, request.Place),
                _ => (false, _localizer[LocalizationKeys.Attachments.InvalidFormat].Value)
            };

            if (!uploaded)
                return Result<string>.Failure(result, StatusCodes.Status400BadRequest);

            await DeleteOldFileAsync(request.MediaType, request.OldFileName!, request.Place);

            return Result<string>.Success(result, _localizer[LocalizationKeys.ActionResults.Updated].Value);
        }

        private Task<bool> DeleteOldFileAsync(MediaType mediaType, string oldFileName, int place)
        {
            return mediaType switch
            {
                MediaType.Image => _imageValidator.DeleteImage(oldFileName, place),
                MediaType.Video => _videoValidator.DeleteVideo(oldFileName, place),
                MediaType.Audio => _audioValidator.DeleteAudio(oldFileName, place),
                MediaType.File => _fileValidator.DeleteFile(oldFileName, place),
                _ => Task.FromResult(false)
            };
        }
    }
}

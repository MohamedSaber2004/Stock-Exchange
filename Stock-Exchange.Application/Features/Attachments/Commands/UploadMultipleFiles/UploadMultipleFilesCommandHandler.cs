using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.Attachments.Commands.UploadMultipleFiles
{
    public class UploadMultipleFilesCommandHandler : IRequestHandler<UploadMultipleFilesCommand, Result<string>>
    {
        private readonly IImageValidator _imageValidator;
        private readonly IVideoValidator _videoValidator;
        private readonly IAudioValidator _audioValidator;
        private readonly IFileValidator _fileValidator;
        private readonly IStringLocalizer<Messages> _localizer;

        public UploadMultipleFilesCommandHandler(
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

        public async Task<Result<string>> Handle(UploadMultipleFilesCommand request, CancellationToken cancellationToken)
        {
            (bool Uploaded, string Result) result = request.MediaType switch
            {
                MediaType.Image => await _imageValidator.UploadMultipleImage(request.Files, request.Place),
                MediaType.Video => await _videoValidator.UploadMultipleVideo(request.Files, request.Place),
                MediaType.File => await _fileValidator.UploadMultipleFile(request.Files, request.Place),
                MediaType.Audio => await UploadMultipleAudioAsync(request.Files!, request.Place),
                _ => (false, _localizer[LocalizationKeys.Attachments.InvalidFormat].Value)
            };

            if (!result.Uploaded)
                return Result<string>.Failure(result.Result, StatusCodes.Status400BadRequest);

            return Result<string>.Success(result.Result, _localizer[LocalizationKeys.ActionResults.Created].Value, StatusCodes.Status201Created);
        }

        private async Task<(bool Uploaded, string Result)> UploadMultipleAudioAsync(List<IFormFile> files, int place)
        {
            var results = new List<string>();
            foreach (var file in files)
            {
                var (uploaded, result) = await _audioValidator.UploadAudio(file, place);
                if (uploaded)
                    results.Add(result);
            }

            if (results.Count == 0)
                return (false, _localizer[LocalizationKeys.Attachments.UploadFailed].Value);

            return (true, string.Join(",", results));
        }
    }
}

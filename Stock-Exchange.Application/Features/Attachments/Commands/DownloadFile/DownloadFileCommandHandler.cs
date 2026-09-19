using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Common.Services;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.Attachments.Commands.DownloadFile
{
    public class DownloadFileCommandHandler : IRequestHandler<DownloadFileCommand, Result<string>>
    {
        private readonly IBaseFileService _baseFileService;
        private readonly IFileValidator _fileValidator;
        private readonly IStringLocalizer<Messages> _localizer;

        public DownloadFileCommandHandler(
            IBaseFileService baseFileService,
            IFileValidator fileValidator,
            IStringLocalizer<Messages> localizer)
        {
            _baseFileService = baseFileService;
            _fileValidator = fileValidator;
            _localizer = localizer;
        }

        public async Task<Result<string>> Handle(DownloadFileCommand request, CancellationToken cancellationToken)
        {
            (bool Success, string Result) result = request.MediaType switch
            {
                MediaType.File => await _fileValidator.DownloadFile(request.FilePlace, request.FileName),
                _ => await _baseFileService.DownloadFileAsync(
                    UploadPaths.GetPath(request.FilePlace) ?? string.Empty,
                    StripPlacePrefix(request.FileName!))
            };

            if (!result.Success)
                return Result<string>.Failure(result.Result, StatusCodes.Status404NotFound);

            return Result<string>.Success(result.Result, _localizer[LocalizationKeys.ActionResults.Ok].Value);
        }

        private static string StripPlacePrefix(string fileName)
        {
            if (fileName.Contains('_'))
            {
                var parts = fileName.Split('_');
                if (int.TryParse(parts[0], out _))
                    return string.Join("_", parts.Skip(1));
            }
            return fileName;
        }
    }
}

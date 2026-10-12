using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Common.Services;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.Attachments.Commands.UploadChunk
{
    public class UploadChunkCommandHandler : IRequestHandler<UploadChunkCommand, Result<UploadChunkResultDto>>
    {
        private static readonly string[] AllowedVideoExtensions = [".mp4", ".avi", ".mkv", ".mov", ".wmv"];
        private static readonly string[] AllowedImageExtensions = [".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp"];
        private static readonly string[] AllowedAudioExtensions = [".mp3", ".wav", ".ogg", ".aac", ".m4a"];

        private readonly IBaseFileService _baseFileService;
        private readonly IStringLocalizer<Messages> _localizer;

        public UploadChunkCommandHandler(
            IBaseFileService baseFileService,
            IStringLocalizer<Messages> localizer)
        {
            _baseFileService = baseFileService;
            _localizer = localizer;
        }

        public async Task<Result<UploadChunkResultDto>> Handle(UploadChunkCommand request, CancellationToken cancellationToken)
        {
            if (request.Chunk == null || request.Chunk.Length == 0)
            {
                return Result<UploadChunkResultDto>.Failure(
                    _localizer[LocalizationKeys.Attachments.FileEmpty].Value,
                    StatusCodes.Status400BadRequest);
            }

            var extension = Path.GetExtension(request.FileName).ToLowerInvariant();
            var isExtensionValid = request.MediaType switch
            {
                MediaType.Video => AllowedVideoExtensions.Contains(extension),
                MediaType.Image => AllowedImageExtensions.Contains(extension),
                MediaType.Audio => AllowedAudioExtensions.Contains(extension),
                MediaType.File => !string.IsNullOrEmpty(extension),
                _ => false
            };

            if (!isExtensionValid)
            {
                var errorKey = request.MediaType switch
                {
                    MediaType.Video => LocalizationKeys.Attachments.InvalidVideoFormat,
                    MediaType.Image => LocalizationKeys.Attachments.InvalidImageFormat,
                    MediaType.Audio => LocalizationKeys.Attachments.InvalidAudioFormat,
                    _ => LocalizationKeys.Attachments.InvalidFormat
                };
                return Result<UploadChunkResultDto>.Failure(_localizer[errorKey].Value, StatusCodes.Status400BadRequest);
            }

            var (saved, _) = await _baseFileService.SaveChunkAsync(request.UploadId, request.ChunkIndex, request.Chunk);
            if (!saved)
            {
                return Result<UploadChunkResultDto>.Failure(
                    _localizer[LocalizationKeys.Attachments.UploadFailed].Value,
                    StatusCodes.Status500InternalServerError);
            }

            var allUploaded = _baseFileService.AreAllChunksUploaded(request.UploadId, request.TotalChunks);
            if (!allUploaded)
            {
                return Result<UploadChunkResultDto>.Success(new UploadChunkResultDto
                {
                    IsCompleted = false,
                    StoredFileName = null,
                    ChunkIndex = request.ChunkIndex,
                    TotalChunks = request.TotalChunks
                }, _localizer[LocalizationKeys.ActionResults.Ok].Value, StatusCodes.Status200OK);
            }

            // All chunks are present -> merge into destination folder
            var folderPath = UploadPaths.GetPath(request.Place) ?? "General";
            var (merged, finalResult) = await _baseFileService.MergeChunksAsync(
                request.UploadId,
                request.TotalChunks,
                request.FileName,
                folderPath);

            if (!merged)
            {
                return Result<UploadChunkResultDto>.Failure(finalResult, StatusCodes.Status400BadRequest);
            }

            var storedFileName = $"{request.Place}_{Path.GetFileName(finalResult)}";
            return Result<UploadChunkResultDto>.Success(new UploadChunkResultDto
            {
                IsCompleted = true,
                StoredFileName = storedFileName,
                ChunkIndex = request.ChunkIndex,
                TotalChunks = request.TotalChunks
            }, _localizer[LocalizationKeys.ActionResults.Created].Value, StatusCodes.Status201Created);
        }
    }
}

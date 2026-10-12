using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Videos.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Repositories.Interfaces;

namespace Stock_Exchange.Application.Features.Videos.Queries.GetVideoById
{
    public class GetVideoByIdQueryHandler : IRequestHandler<GetVideoByIdQuery, Result<VideoDto>>
    {
        private readonly IVideoRepository _videoRepository;
        private readonly ICurrentLanguageService _currentLanguageService;

        public GetVideoByIdQueryHandler(
            IVideoRepository videoRepository,
            ICurrentLanguageService currentLanguageService)
        {
            _videoRepository = videoRepository;
            _currentLanguageService = currentLanguageService;
        }

        public async Task<Result<VideoDto>> Handle(GetVideoByIdQuery request, CancellationToken cancellationToken)
        {
            var video = await _videoRepository
                .GetAllAsync(v => v.Id == request.Id)
                .AsNoTracking()
                .Select(v => new VideoDto
                {
                    Id = v.Id,
                    TitleEn = v.TitleEn,
                    TitleAr = v.TitleAr,
                    DescriptionEn = v.DescriptionEn,
                    DescriptionAr = v.DescriptionAr,
                    ThumbnailUrl = v.ThumbnailUrl,
                    VideoUrl = v.VideoUrl,
                    DurationSeconds = v.DurationSeconds,
                    InstructorName = v.InstructorName,
                    CategoryEn = v.CategoryEn,
                    CategoryAr = v.CategoryAr,
                    IsPreviewable = v.IsPreviewable,
                    IsFeaturedOnHome = v.IsFeaturedOnHome,
                    DisplayOrder = v.DisplayOrder,
                    IsActive = v.IsActive,
                    CreatedAt = v.CreatedAt,
                    CategoryId = v.CategoryId,
                    VideoCategoryId = v.CategoryId,
                    CategoryEnName = v.Category != null ? v.Category.CategoryEnName : null,
                    CategoryArName = v.Category != null ? v.Category.CategoryArName : null
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (video is null)
                return Result<VideoDto>.Failure(LocalizationKeys.VideoMessages.VideoNotFound, StatusCodes.Status404NotFound);

            List<VideoDto> relatedVideos = new();

            var categoryId = video.CategoryId;
            var categoryEn = !string.IsNullOrWhiteSpace(video.CategoryEnName) ? video.CategoryEnName : video.CategoryEn;
            var categoryAr = !string.IsNullOrWhiteSpace(video.CategoryArName) ? video.CategoryArName : video.CategoryAr;

            var hasCategoryId = categoryId.HasValue;
            var hasCategoryEn = !string.IsNullOrWhiteSpace(categoryEn);
            var hasCategoryAr = !string.IsNullOrWhiteSpace(categoryAr);

            if (hasCategoryId || hasCategoryEn || hasCategoryAr)
            {
                relatedVideos = await _videoRepository
                    .GetAllAsync(v =>
                        v.Id != request.Id &&
                        v.IsActive &&
                        (v.Category == null || v.Category.IsActive) &&
                        (
                            (hasCategoryId && v.CategoryId.HasValue && v.CategoryId.Value == categoryId!.Value) ||
                            (hasCategoryEn && (v.CategoryEn == categoryEn || (v.Category != null && v.Category.CategoryEnName == categoryEn))) ||
                            (hasCategoryAr && (v.CategoryAr == categoryAr || (v.Category != null && v.Category.CategoryArName == categoryAr)))
                        ))
                    .AsNoTracking()
                    .OrderBy(v => v.DisplayOrder)
                    .ThenByDescending(v => v.CreatedAt)
                    .Take(6)
                    .Select(v => new VideoDto
                    {
                        Id = v.Id,
                        TitleEn = v.TitleEn,
                        TitleAr = v.TitleAr,
                        DescriptionEn = v.DescriptionEn,
                        DescriptionAr = v.DescriptionAr,
                        ThumbnailUrl = v.ThumbnailUrl,
                        VideoUrl = v.VideoUrl,
                        DurationSeconds = v.DurationSeconds,
                        InstructorName = v.InstructorName,
                        CategoryEn = v.CategoryEn,
                        CategoryAr = v.CategoryAr,
                        CategoryId = v.CategoryId,
                        VideoCategoryId = v.CategoryId,
                        CategoryEnName = v.Category != null ? v.Category.CategoryEnName : null,
                        CategoryArName = v.Category != null ? v.Category.CategoryArName : null,
                        IsPreviewable = v.IsPreviewable,
                        IsFeaturedOnHome = v.IsFeaturedOnHome,
                        DisplayOrder = v.DisplayOrder,
                        IsActive = v.IsActive,
                        CreatedAt = v.CreatedAt
                    })
                    .ToListAsync(cancellationToken);
            }

            video.RelatedVideos = relatedVideos;

            if (request.ApplyLanguageFilter ?? true)
            {
                video.ApplyLanguageFilter(_currentLanguageService.Language);
            }

            return Result<VideoDto>.Success(video);
        }
    }
}

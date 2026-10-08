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

            var relatedBaseQuery = _videoRepository
                .GetAllAsync(v => v.Id != request.Id)
                .AsNoTracking();

            List<VideoDto> relatedVideos = new();

            if (video.CategoryId.HasValue)
            {
                var categoryId = video.CategoryId.Value;
                relatedVideos = await relatedBaseQuery
                    .Where(v => v.IsActive && v.CategoryId == categoryId)
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
            else if (!string.IsNullOrWhiteSpace(video.CategoryEn))
            {
                var catEn = video.CategoryEn;
                relatedVideos = await relatedBaseQuery
                    .Where(v => v.IsActive && v.CategoryEn == catEn)
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

            if (relatedVideos.Count < 6)
            {
                var existingIds = relatedVideos.Select(r => r.Id).ToList();
                var remainingCount = 6 - relatedVideos.Count;

                var fallbackActive = await relatedBaseQuery
                    .Where(v => v.IsActive && !existingIds.Contains(v.Id))
                    .OrderBy(v => v.DisplayOrder)
                    .ThenByDescending(v => v.CreatedAt)
                    .Take(remainingCount)
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

                relatedVideos.AddRange(fallbackActive);
            }

            if (relatedVideos.Count < 6)
            {
                var existingIds = relatedVideos.Select(r => r.Id).ToList();
                var remainingCount = 6 - relatedVideos.Count;

                var fallbackAny = await relatedBaseQuery
                    .Where(v => !existingIds.Contains(v.Id))
                    .OrderBy(v => v.DisplayOrder)
                    .ThenByDescending(v => v.CreatedAt)
                    .Take(remainingCount)
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

                relatedVideos.AddRange(fallbackAny);
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

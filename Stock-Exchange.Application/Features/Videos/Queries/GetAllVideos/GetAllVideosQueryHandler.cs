using MediatR;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Extensions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Videos.DTOs;
using Stock_Exchange.Domain.Repositories.Interfaces;

namespace Stock_Exchange.Application.Features.Videos.Queries.GetAllVideos
{
    public class GetAllVideosQueryHandler : IRequestHandler<GetAllVideosQuery, Result<PagginatedResult<VideoDto>>>
    {
        private readonly IVideoRepository _videoRepository;
        private readonly ICurrentLanguageService _currentLanguageService;

        public GetAllVideosQueryHandler(
            IVideoRepository videoRepository,
            ICurrentLanguageService currentLanguageService)
        {
            _videoRepository = videoRepository;
            _currentLanguageService = currentLanguageService;
        }

        public async Task<Result<PagginatedResult<VideoDto>>> Handle(GetAllVideosQuery request, CancellationToken cancellationToken)
        {
            var query = _videoRepository.GetAllAsync(null);

            if (request.IsActive.HasValue)
            {
                query = query.Where(v => v.IsActive == request.IsActive.Value);
            }
            

            if (request.CategoryId.HasValue && request.CategoryId.Value != Guid.Empty)
            {
                query = query.Where(v => v.CategoryId == request.CategoryId.Value);
            }

            var search = request.Search?.Trim();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                query = query.Where(v =>
                    v.TitleEn.ToLower().Contains(term) ||
                    v.TitleAr.ToLower().Contains(term) ||
                    v.InstructorName.ToLower().Contains(term) ||
                    v.CategoryEn.ToLower().Contains(term) ||
                    v.CategoryAr.ToLower().Contains(term) ||
                    (v.Category != null && (v.Category.CategoryEnName.ToLower().Contains(term) || v.Category.CategoryArName.ToLower().Contains(term))));
            }

            var safePageSize = request.PageSize <= 0 ? 10 : request.PageSize;
            var safePageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;

            var pagedVideos = await query
                .AsNoTracking()
                .OrderBy(v => v.DisplayOrder)
                .ThenByDescending(v => v.CreatedAt)
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
                .AsPagginatedListAsync(safePageNumber, safePageSize, cancellationToken);

            if (pagedVideos.Items.Any())
            {
                var categoryIds = pagedVideos.Items
                    .Where(v => v.CategoryId.HasValue)
                    .Select(v => v.CategoryId!.Value)
                    .Distinct()
                    .ToList();

                var categoryEns = pagedVideos.Items
                    .Select(v => !string.IsNullOrWhiteSpace(v.CategoryEnName) ? v.CategoryEnName : v.CategoryEn)
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .Distinct()
                    .ToList();

                var categoryArs = pagedVideos.Items
                    .Select(v => !string.IsNullOrWhiteSpace(v.CategoryArName) ? v.CategoryArName : v.CategoryAr)
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .Distinct()
                    .ToList();

                var hasCategoryIds = categoryIds.Count > 0;
                var hasCategoryEns = categoryEns.Count > 0;
                var hasCategoryArs = categoryArs.Count > 0;

                if (hasCategoryIds || hasCategoryEns || hasCategoryArs)
                {
                    var relatedQuery = _videoRepository.GetAllAsync(v => v.IsActive && (v.Category == null || v.Category.IsActive));

                    relatedQuery = relatedQuery.Where(v =>
                        (hasCategoryIds && v.CategoryId.HasValue && categoryIds.Contains(v.CategoryId.Value)) ||
                        (hasCategoryEns && (categoryEns.Contains(v.CategoryEn) || (v.Category != null && categoryEns.Contains(v.Category.CategoryEnName)))) ||
                        (hasCategoryArs && (categoryArs.Contains(v.CategoryAr) || (v.Category != null && categoryArs.Contains(v.Category.CategoryArName)))));

                    var relatedCandidates = await relatedQuery
                        .AsNoTracking()
                        .OrderBy(v => v.DisplayOrder)
                        .ThenByDescending(v => v.CreatedAt)
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

                    foreach (var video in pagedVideos.Items)
                    {
                        var videoCatEn = !string.IsNullOrWhiteSpace(video.CategoryEnName) ? video.CategoryEnName : video.CategoryEn;
                        var videoCatAr = !string.IsNullOrWhiteSpace(video.CategoryArName) ? video.CategoryArName : video.CategoryAr;

                        var matching = relatedCandidates.Where(r =>
                            r.Id != video.Id &&
                            (
                                (video.CategoryId.HasValue && r.CategoryId.HasValue && r.CategoryId.Value == video.CategoryId.Value) ||
                                (!string.IsNullOrWhiteSpace(videoCatEn) && (r.CategoryEn == videoCatEn || r.CategoryEnName == videoCatEn)) ||
                                (!string.IsNullOrWhiteSpace(videoCatAr) && (r.CategoryAr == videoCatAr || r.CategoryArName == videoCatAr))
                            ));

                        video.RelatedVideos = matching
                            .Take(6)
                            .Select(r => new VideoDto
                            {
                                Id = r.Id,
                                TitleEn = r.TitleEn,
                                TitleAr = r.TitleAr,
                                DescriptionEn = r.DescriptionEn,
                                DescriptionAr = r.DescriptionAr,
                                ThumbnailUrl = r.ThumbnailUrl,
                                VideoUrl = r.VideoUrl,
                                DurationSeconds = r.DurationSeconds,
                                InstructorName = r.InstructorName,
                                CategoryEn = r.CategoryEn,
                                CategoryAr = r.CategoryAr,
                                CategoryId = r.CategoryId,
                                VideoCategoryId = r.VideoCategoryId,
                                CategoryEnName = r.CategoryEnName,
                                CategoryArName = r.CategoryArName,
                                IsPreviewable = r.IsPreviewable,
                                IsFeaturedOnHome = r.IsFeaturedOnHome,
                                DisplayOrder = r.DisplayOrder,
                                IsActive = r.IsActive,
                                CreatedAt = r.CreatedAt
                            })
                            .ToList();
                    }
                }
            }

            if (request.ApplyLanguageFilter)
            {
                var language = _currentLanguageService.Language;
                foreach (var item in pagedVideos.Items)
                {
                    item.ApplyLanguageFilter(language);
                }
            }

            return Result<PagginatedResult<VideoDto>>.Success(pagedVideos);
        }
    }
}

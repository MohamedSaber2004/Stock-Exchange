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
            

            var videoCategoryId = request.VideoCategoryId ?? request.CategoryId;
            if (videoCategoryId.HasValue && videoCategoryId.Value != Guid.Empty)
            {
                query = query.Where(v => v.CategoryId == videoCategoryId.Value);
            }

            var category = request.Category?.Trim();
            if (!string.IsNullOrWhiteSpace(category))
            {
                var catTerm = category.ToLower();
                query = query.Where(v =>
                    v.CategoryEn.ToLower() == catTerm ||
                    v.CategoryAr.ToLower() == catTerm ||
                    (v.Category != null && (v.Category.CategoryEnName.ToLower() == catTerm || v.Category.CategoryArName.ToLower() == catTerm)));
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

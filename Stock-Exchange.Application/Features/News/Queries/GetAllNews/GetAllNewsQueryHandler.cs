using MediatR;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Extensions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.News.DTOs;
using Stock_Exchange.Domain.Repositories.Interfaces;

namespace Stock_Exchange.Application.Features.News.Queries.GetAllNews
{
    public class GetAllNewsQueryHandler : IRequestHandler<GetAllNewsQuery, Result<PagginatedResult<NewsDto>>>
    {
        private readonly INewsRepository _newsRepository;
        private readonly ICurrentLanguageService _currentLanguageService;

        public GetAllNewsQueryHandler(
            INewsRepository newsRepository,
            ICurrentLanguageService currentLanguageService)
        {
            _newsRepository = newsRepository;
            _currentLanguageService = currentLanguageService;
        }

        public async Task<Result<PagginatedResult<NewsDto>>> Handle(GetAllNewsQuery request, CancellationToken cancellationToken)
        {
            var query = _newsRepository.GetAllAsync(null);

            if (request.IsActive.HasValue)
            {
                query = query.Where(n => n.IsActive == request.IsActive.Value);
            }

            if (request.IsFeaturedOnHome.HasValue)
            {
                query = query.Where(n => n.IsFeaturedOnHome == request.IsFeaturedOnHome.Value);
            }

            var cat = request.Category?.Trim();
            if (!string.IsNullOrWhiteSpace(cat))
            {
                var catLower = cat.ToLower();
                query = query.Where(n => n.CategoryEn.ToLower() == catLower || n.CategoryAr.ToLower() == catLower);
            }

            var search = request.Search?.Trim();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                query = query.Where(n =>
                    n.TitleEn.ToLower().Contains(term) ||
                    n.TitleAr.ToLower().Contains(term) ||
                    n.SummaryEn.ToLower().Contains(term) ||
                    n.SummaryAr.ToLower().Contains(term) ||
                    n.CategoryEn.ToLower().Contains(term) ||
                    n.CategoryAr.ToLower().Contains(term));
            }

            var safePageSize = request.PageSize <= 0 ? 10 : request.PageSize;
            var safePageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;

            var paged = await query
                .AsNoTracking()
                .OrderBy(n => n.DisplayOrder)
                .ThenByDescending(n => n.PublishedAt)
                .Select(n => new NewsDto
                {
                    Id = n.Id,
                    TitleEn = n.TitleEn,
                    TitleAr = n.TitleAr,
                    SummaryEn = n.SummaryEn,
                    SummaryAr = n.SummaryAr,
                    ImageUrl = n.ImageUrl,
                    CategoryEn = n.CategoryEn,
                    CategoryAr = n.CategoryAr,
                    PublishedAt = n.PublishedAt,
                    DisplayOrder = n.DisplayOrder,
                    IsFeaturedOnHome = n.IsFeaturedOnHome,
                    IsActive = n.IsActive,
                    CreatedAt = n.CreatedAt
                })
                .AsPagginatedListAsync(safePageNumber, safePageSize, cancellationToken);

            if (request.ApplyLanguageFilter)
            {
                var language = _currentLanguageService.Language;
                foreach (var item in paged.Items)
                {
                    item.ApplyLanguageFilter(language);
                }
            }

            return Result<PagginatedResult<NewsDto>>.Success(paged);
        }
    }
}

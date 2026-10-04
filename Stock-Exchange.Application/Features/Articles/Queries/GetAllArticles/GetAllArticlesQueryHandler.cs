using MediatR;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Extensions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Articles.DTOs;
using Stock_Exchange.Domain.Repositories.Interfaces;

namespace Stock_Exchange.Application.Features.Articles.Queries.GetAllArticles
{
    public class GetAllArticlesQueryHandler : IRequestHandler<GetAllArticlesQuery, Result<PagginatedResult<ArticleDto>>>
    {
        private readonly IArticleRepository _articleRepository;
        private readonly ICurrentLanguageService _currentLanguageService;

        public GetAllArticlesQueryHandler(
            IArticleRepository articleRepository,
            ICurrentLanguageService currentLanguageService)
        {
            _articleRepository = articleRepository;
            _currentLanguageService = currentLanguageService;
        }

        public async Task<Result<PagginatedResult<ArticleDto>>> Handle(GetAllArticlesQuery request, CancellationToken cancellationToken)
        {
            var query = _articleRepository.GetAllAsync(null);

            if (request.IsActive.HasValue)
            {
                query = query.Where(a => a.IsActive == request.IsActive.Value);
            }
            

            var categoryId = request.ArticleCategoryId ?? request.CategoryId;
            if (categoryId.HasValue && categoryId.Value != Guid.Empty)
            {
                query = query.Where(a => a.CategoryId == categoryId.Value);
            }

            var search = request.Search?.Trim();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                query = query.Where(a =>
                    a.TitleEn.ToLower().Contains(term) ||
                    a.TitleAr.ToLower().Contains(term) ||
                    a.ExcerptEn.ToLower().Contains(term) ||
                    a.ExcerptAr.ToLower().Contains(term) ||
                    a.AuthorName.ToLower().Contains(term));
            }

            var safePageSize = request.PageSize <= 0 ? 10 : request.PageSize;
            var safePageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;

            var pagedArticles = await query
                .AsNoTracking()
                .OrderBy(a => a.DisplayOrder)
                .ThenByDescending(a => a.PublishedAt)
                .Select(a => new ArticleDto
                {
                    Id = a.Id,
                    TitleEn = a.TitleEn,
                    TitleAr = a.TitleAr,
                    ExcerptEn = a.ExcerptEn,
                    ExcerptAr = a.ExcerptAr,
                    ImageUrl = a.ImageUrl,
                    AuthorName = a.AuthorName,
                    PublishedAt = a.PublishedAt,
                    IsFeaturedOnHome = a.IsFeaturedOnHome,
                    DisplayOrder = a.DisplayOrder,
                    IsActive = a.IsActive,
                    CreatedAt = a.CreatedAt,
                    CategoryId = a.CategoryId,
                    ArticleCategoryId = a.CategoryId,
                    CategoryEnName = a.Category != null ? a.Category.CategoryEnName : null,
                    CategoryArName = a.Category != null ? a.Category.CategoryArName : null
                })
                .AsPagginatedListAsync(safePageNumber, safePageSize, cancellationToken);

            if (request.ApplyLanguageFilter)
            {
                var language = _currentLanguageService.Language;
                foreach (var item in pagedArticles.Items)
                {
                    item.ApplyLanguageFilter(language);
                }
            }

            return Result<PagginatedResult<ArticleDto>>.Success(pagedArticles);
        }
    }
}

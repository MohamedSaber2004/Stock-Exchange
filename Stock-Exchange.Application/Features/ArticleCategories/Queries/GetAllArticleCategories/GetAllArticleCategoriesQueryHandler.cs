using MediatR;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.ArticleCategories.DTOs;
using Stock_Exchange.Domain.Repositories.Interfaces;

namespace Stock_Exchange.Application.Features.ArticleCategories.Queries.GetAllArticleCategories
{
    public class GetAllArticleCategoriesQueryHandler : IRequestHandler<GetAllArticleCategoriesQuery, Result<List<ArticleCategoryDto>>>
    {
        private readonly IArticleCategoryRepository _categoryRepository;
        private readonly ICurrentLanguageService _currentLanguageService;

        public GetAllArticleCategoriesQueryHandler(
            IArticleCategoryRepository categoryRepository,
            ICurrentLanguageService currentLanguageService)
        {
            _categoryRepository = categoryRepository;
            _currentLanguageService = currentLanguageService;
        }

        public async Task<Result<List<ArticleCategoryDto>>> Handle(GetAllArticleCategoriesQuery request, CancellationToken cancellationToken)
        {
            var query = _categoryRepository.GetAllAsync(null);

            if (request.IsActive.HasValue)
            {
                query = query.Where(c => c.IsActive == request.IsActive.Value);
            }
            else
            {
                query = query.Where(c => c.IsActive);
            }

            var search = request.Search?.Trim();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                query = query.Where(c =>
                    c.CategoryEnName.ToLower().Contains(term) ||
                    c.CategoryArName.ToLower().Contains(term));
            }

            var categories = await query
                .AsNoTracking()
                .OrderBy(c => c.CategoryEnName)
                .Select(c => new ArticleCategoryDto
                {
                    Id = c.Id,
                    CategoryArName = c.CategoryArName,
                    CategoryEnName = c.CategoryEnName,
                    ArticlesCount = c.Articles.Count(a => !a.IsDeleted && a.IsActive),
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync(cancellationToken);

            if (request.ApplyLanguageFilter ?? false)
            {
                var language = _currentLanguageService.Language;
                foreach (var category in categories)
                {
                    category.ApplyLanguageFilter(language);
                }
            }

            return Result<List<ArticleCategoryDto>>.Success(categories);
        }
    }
}

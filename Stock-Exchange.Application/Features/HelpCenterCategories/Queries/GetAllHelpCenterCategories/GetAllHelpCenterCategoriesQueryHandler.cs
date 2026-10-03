using MediatR;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.HelpCenterCategories.DTOs;
using Stock_Exchange.Domain.Repositories.Interfaces;

namespace Stock_Exchange.Application.Features.HelpCenterCategories.Queries.GetAllHelpCenterCategories
{
    public class GetAllHelpCenterCategoriesQueryHandler : IRequestHandler<GetAllHelpCenterCategoriesQuery, Result<List<HelpCenterCategoryDto>>>
    {
        private readonly IHelpCenterCategoryRepository _categoryRepository;
        private readonly ICurrentLanguageService _currentLanguageService;

        public GetAllHelpCenterCategoriesQueryHandler(
            IHelpCenterCategoryRepository categoryRepository,
            ICurrentLanguageService currentLanguageService)
        {
            _categoryRepository = categoryRepository;
            _currentLanguageService = currentLanguageService;
        }

        public async Task<Result<List<HelpCenterCategoryDto>>> Handle(GetAllHelpCenterCategoriesQuery request, CancellationToken cancellationToken)
        {
            var query = _categoryRepository.GetAllAsync(c => c.IsActive);

            var search = request.Search?.Trim();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                query = query.Where(c =>
                    c.TitleEn.ToLower().Contains(term) ||
                    c.TitleAr.ToLower().Contains(term));
            }

            var categories = await query
                .AsNoTracking()
                .OrderBy(c => c.TitleEn)
                .Select(c => new HelpCenterCategoryDto
                {
                    Id = c.Id,
                    TitleEn = c.TitleEn,
                    TitleAr = c.TitleAr
                })
                .ToListAsync(cancellationToken);

            if (request.ApplyLanguageFilter ?? true)
            {
                var language = _currentLanguageService.Language;
                foreach (var category in categories)
                {
                    category.ApplyLanguageFilter(language);
                }
            }

            return Result<List<HelpCenterCategoryDto>>.Success(categories);
        }
    }
}

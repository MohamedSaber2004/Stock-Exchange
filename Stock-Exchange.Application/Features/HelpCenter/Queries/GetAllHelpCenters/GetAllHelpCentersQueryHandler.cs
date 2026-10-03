using MediatR;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.HelpCenter.DTOs;
using Stock_Exchange.Domain.Repositories.Interfaces;

namespace Stock_Exchange.Application.Features.HelpCenter.Queries.GetAllHelpCenters
{
    public class GetAllHelpCentersQueryHandler : IRequestHandler<GetAllHelpCentersQuery, Result<List<HelpCenterDto>>>
    {
        private readonly IHelpCenterRepository _helpCenterRepository;
        private readonly ICurrentLanguageService _currentLanguageService;

        public GetAllHelpCentersQueryHandler(
            IHelpCenterRepository helpCenterRepository,
            ICurrentLanguageService currentLanguageService)
        {
            _helpCenterRepository = helpCenterRepository;
            _currentLanguageService = currentLanguageService;
        }

        public async Task<Result<List<HelpCenterDto>>> Handle(GetAllHelpCentersQuery request, CancellationToken cancellationToken)
        {
            var query = _helpCenterRepository.GetAllAsync(h => h.IsActive);

            if (request.CategoryId.HasValue)
            {
                var categoryId = request.CategoryId.Value;
                query = query.Where(h => h.CategoryId == categoryId);
            }

            var search = request.Search?.Trim();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                query = query.Where(h =>
                    h.TitleEn.ToLower().Contains(term) ||
                    h.TitleAr.ToLower().Contains(term) ||
                    h.ContentEn.ToLower().Contains(term) ||
                    h.ContentAr.ToLower().Contains(term));
            }

            var helpCenters = await query
                .AsNoTracking()
                .OrderBy(h => h.DisplayOrder)
                .ThenBy(h => h.TitleEn)
                .Select(h => new HelpCenterDto
                {
                    Id = h.Id,
                    TitleEn = h.TitleEn,
                    TitleAr = h.TitleAr,
                    ContentEn = h.ContentEn,
                    ContentAr = h.ContentAr,
                    CategoryId = h.CategoryId,
                    DisplayOrder = h.DisplayOrder
                })
                .ToListAsync(cancellationToken);

            if (request.ApplyLanguageFilter ?? true)
            {
                var language = _currentLanguageService.Language;
                foreach (var helpCenter in helpCenters)
                    helpCenter.ApplyLanguageFilter(language);
            }

            return Result<List<HelpCenterDto>>.Success(helpCenters);
        }
    }
}

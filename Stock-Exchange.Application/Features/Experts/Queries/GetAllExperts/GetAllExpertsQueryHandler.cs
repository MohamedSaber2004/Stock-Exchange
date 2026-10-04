using MediatR;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Extensions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Experts.DTOs;
using Stock_Exchange.Domain.Repositories.Interfaces;

namespace Stock_Exchange.Application.Features.Experts.Queries.GetAllExperts
{
    public class GetAllExpertsQueryHandler : IRequestHandler<GetAllExpertsQuery, Result<PagginatedResult<ExpertDto>>>
    {
        private readonly IExpertRepository _expertRepository;
        private readonly ICurrentLanguageService _currentLanguageService;

        public GetAllExpertsQueryHandler(
            IExpertRepository expertRepository,
            ICurrentLanguageService currentLanguageService)
        {
            _expertRepository = expertRepository;
            _currentLanguageService = currentLanguageService;
        }

        public async Task<Result<PagginatedResult<ExpertDto>>> Handle(GetAllExpertsQuery request, CancellationToken cancellationToken)
        {
            var query = _expertRepository.GetAllAsync(null);

            if (request.IsActive.HasValue)
            {
                query = query.Where(e => e.IsActive == request.IsActive.Value);
            }

            if (request.IsFeaturedOnHome.HasValue)
            {
                query = query.Where(e => e.IsFeaturedOnHome == request.IsFeaturedOnHome.Value);
            }

            var search = request.Search?.Trim();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                query = query.Where(e =>
                    e.FullNameEn.ToLower().Contains(term) ||
                    e.FullNameAr.ToLower().Contains(term) ||
                    e.TitleEn.ToLower().Contains(term) ||
                    e.TitleAr.ToLower().Contains(term));
            }

            var safePageSize = request.PageSize <= 0 ? 20 : request.PageSize;
            var safePageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;

            var paged = await query
                .AsNoTracking()
                .OrderBy(e => e.DisplayOrder)
                .ThenByDescending(e => e.CreatedAt)
                .Select(e => new ExpertDto
                {
                    Id = e.Id,
                    FullNameEn = e.FullNameEn,
                    FullNameAr = e.FullNameAr,
                    TitleEn = e.TitleEn,
                    TitleAr = e.TitleAr,
                    AvatarUrl = e.AvatarUrl,
                    DisplayOrder = e.DisplayOrder,
                    IsFeaturedOnHome = e.IsFeaturedOnHome,
                    IsActive = e.IsActive,
                    CreatedAt = e.CreatedAt
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

            return Result<PagginatedResult<ExpertDto>>.Success(paged);
        }
    }
}

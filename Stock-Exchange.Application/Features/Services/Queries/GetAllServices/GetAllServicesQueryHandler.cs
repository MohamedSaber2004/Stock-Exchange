using MediatR;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Extensions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Services.DTOs;
using Stock_Exchange.Domain.Repositories.Interfaces;

namespace Stock_Exchange.Application.Features.Services.Queries.GetAllServices
{
    public class GetAllServicesQueryHandler : IRequestHandler<GetAllServicesQuery, Result<PagginatedResult<ServiceDto>>>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly ICurrentLanguageService _currentLanguageService;

        public GetAllServicesQueryHandler(
            IServiceRepository serviceRepository,
            ICurrentLanguageService currentLanguageService)
        {
            _serviceRepository = serviceRepository;
            _currentLanguageService = currentLanguageService;
        }

        public async Task<Result<PagginatedResult<ServiceDto>>> Handle(GetAllServicesQuery request, CancellationToken cancellationToken)
        {
            var query = _serviceRepository.GetAllAsync(null);

            if (request.IsActive.HasValue)
            {
                query = query.Where(s => s.IsActive == request.IsActive.Value);
            }

            var search = request.Search?.Trim();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                query = query.Where(s =>
                    s.TitleEn.ToLower().Contains(term) ||
                    s.TitleAr.ToLower().Contains(term) ||
                    s.DescriptionEn.ToLower().Contains(term) ||
                    s.DescriptionAr.ToLower().Contains(term));
            }

            var safePageSize = request.PageSize <= 0 ? 20 : request.PageSize;
            var safePageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;

            var paged = await query
                .AsNoTracking()
                .OrderBy(s => s.DisplayOrder)
                .ThenByDescending(s => s.CreatedAt)
                .Select(s => new ServiceDto
                {
                    Id = s.Id,
                    TitleEn = s.TitleEn,
                    TitleAr = s.TitleAr,
                    DescriptionEn = s.DescriptionEn,
                    DescriptionAr = s.DescriptionAr,
                    IconName = s.IconName,
                    ImageUrl = s.ImageUrl,
                    LinkRoute = s.LinkRoute,
                    DisplayOrder = s.DisplayOrder,
                    IsActive = s.IsActive,
                    CreatedAt = s.CreatedAt
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

            return Result<PagginatedResult<ServiceDto>>.Success(paged);
        }
    }
}

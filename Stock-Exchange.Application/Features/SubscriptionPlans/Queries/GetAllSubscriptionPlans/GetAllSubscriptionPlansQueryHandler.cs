using MediatR;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.SubscriptionPlans.DTOs;
using Stock_Exchange.Domain.Enums;
using Stock_Exchange.Domain.Repositories.Interfaces;

namespace Stock_Exchange.Application.Features.SubscriptionPlans.Queries.GetAllSubscriptionPlans
{
    public class GetAllSubscriptionPlansQueryHandler : IRequestHandler<GetAllSubscriptionPlansQuery, Result<List<SubscriptionPlanDto>>>
    {
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
        private readonly ICurrentLanguageService _currentLanguageService;

        public GetAllSubscriptionPlansQueryHandler(
            ISubscriptionPlanRepository subscriptionPlanRepository,
            ICurrentLanguageService currentLanguageService)
        {
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _currentLanguageService = currentLanguageService;
        }

        public async Task<Result<List<SubscriptionPlanDto>>> Handle(GetAllSubscriptionPlansQuery request, CancellationToken cancellationToken)
        {
            var query = _subscriptionPlanRepository.GetAllWithIncluding(
                p => request.IsActive.HasValue ? p.IsActive == request.IsActive.Value : p.IsActive,
                p => p.Features);

            var plans = await query
                .AsNoTracking()
                .OrderBy(p => p.DisplayOrder)
                .Select(p => new SubscriptionPlanDto
                {
                    Id = p.Id,
                    NameEn = p.NameEn,
                    NameAr = p.NameAr,
                    PriceEgp = p.PriceEgp,
                    Period = p.Period == SubscriptionPeriod.Yearly ? "yr" : "mo",
                    IsHighlighted = p.IsHighlighted,
                    DisplayOrder = p.DisplayOrder,
                    IsActive = p.IsActive,
                    FeatureItems = p.Features
                        .OrderBy(f => f.DisplayOrder)
                        .Select(f => new PlanFeatureDto
                        {
                            Id = f.Id,
                            TextEn = f.TextEn,
                            TextAr = f.TextAr,
                            DisplayOrder = f.DisplayOrder
                        })
                        .ToList(),
                    Features = p.Features
                        .OrderBy(f => f.DisplayOrder)
                        .Select(f => f.TextEn)
                        .ToList()
                })
                .ToListAsync(cancellationToken);

            if (request.ApplyLanguageFilter)
            {
                var language = _currentLanguageService.Language;
                foreach (var plan in plans)
                {
                    plan.ApplyLanguageFilter(language);
                }
            }

            return Result<List<SubscriptionPlanDto>>.Success(plans);
        }
    }
}

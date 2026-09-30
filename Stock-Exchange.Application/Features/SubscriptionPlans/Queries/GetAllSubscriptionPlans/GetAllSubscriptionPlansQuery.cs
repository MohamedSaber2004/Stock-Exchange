using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.SubscriptionPlans.DTOs;

namespace Stock_Exchange.Application.Features.SubscriptionPlans.Queries.GetAllSubscriptionPlans
{
    public class GetAllSubscriptionPlansQuery : IRequest<Result<List<SubscriptionPlanDto>>>
    {
        public bool? IsActive { get; set; }
        public bool ApplyLanguageFilter { get; set; } = true;

        public GetAllSubscriptionPlansQuery()
        {
        }

        public GetAllSubscriptionPlansQuery(bool? isActive = null, bool applyLanguageFilter = true)
        {
            IsActive = isActive;
            ApplyLanguageFilter = applyLanguageFilter;
        }
    }
}

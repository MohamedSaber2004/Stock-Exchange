using Stock_Exchange.Domain.Common;
using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Domain.Entities
{
    public class SubscriptionPlan : BaseEntity<Guid>
    {
        public string NameEn { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public decimal PriceEgp { get; set; }
        public SubscriptionPeriod Period { get; set; } = SubscriptionPeriod.Monthly;
        public bool IsHighlighted { get; set; }
        public int DisplayOrder { get; set; }

        public virtual ICollection<PlanFeature> Features { get; set; } = new List<PlanFeature>();
    }
}

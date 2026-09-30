using Stock_Exchange.Domain.Common;

namespace Stock_Exchange.Domain.Entities
{
    public class PlanFeature : BaseEntity<Guid>
    {
        public Guid PlanId { get; set; }
        public string TextEn { get; set; } = string.Empty;
        public string TextAr { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }

        public virtual SubscriptionPlan Plan { get; set; } = default!;
    }
}

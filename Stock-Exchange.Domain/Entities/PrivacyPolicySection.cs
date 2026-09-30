using Stock_Exchange.Domain.Common;

namespace Stock_Exchange.Domain.Entities
{
    public class PrivacyPolicySection : BaseEntity<Guid>
    {
        public Guid PrivacyPolicyId { get; set; }

        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string ContentEn { get; set; } = string.Empty;
        public string ContentAr { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }

        public virtual PrivacyPolicy PrivacyPolicy { get; set; } = default!;
    }
}

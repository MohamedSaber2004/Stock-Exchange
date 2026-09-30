using Stock_Exchange.Domain.Common;

namespace Stock_Exchange.Domain.Entities
{
    public class TermsAndConditionsSection : BaseEntity<Guid>
    {
        public Guid TermsAndConditionsId { get; set; }

        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string ContentEn { get; set; } = string.Empty;
        public string ContentAr { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }

        public virtual TermsAndConditions TermsAndConditions { get; set; } = default!;
    }
}

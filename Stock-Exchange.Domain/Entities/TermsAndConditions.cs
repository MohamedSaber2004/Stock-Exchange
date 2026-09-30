using Stock_Exchange.Domain.Common;

namespace Stock_Exchange.Domain.Entities
{
    public class TermsAndConditions : BaseEntity<Guid>
    {
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }

        public virtual ICollection<TermsAndConditionsSection> Sections { get; set; } = new List<TermsAndConditionsSection>();
    }
}

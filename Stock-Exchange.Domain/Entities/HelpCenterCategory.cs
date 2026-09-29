using Stock_Exchange.Domain.Common;

namespace Stock_Exchange.Domain.Entities
{
    public class HelpCenterCategory : BaseEntity<Guid>
    {
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }

        public virtual ICollection<HelpCenter> HelpCenters { get; set; } = new List<HelpCenter>();
    }
}

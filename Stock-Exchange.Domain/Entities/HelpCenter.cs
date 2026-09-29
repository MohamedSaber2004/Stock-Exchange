using Stock_Exchange.Domain.Common;

namespace Stock_Exchange.Domain.Entities
{
    public class HelpCenter : BaseEntity<Guid>
    {
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string ContentEn { get; set; } = string.Empty;
        public string ContentAr { get; set; } = string.Empty;
        public Guid? CategoryId { get; set; }
        public int DisplayOrder { get; set; }

        public virtual HelpCenterCategory? Category { get; set; }
    }
}

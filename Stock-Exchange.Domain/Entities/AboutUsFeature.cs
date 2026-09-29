using Stock_Exchange.Domain.Common;

namespace Stock_Exchange.Domain.Entities
{
    public class AboutUsFeature : BaseEntity<Guid>
    {
        public Guid AboutUsId { get; set; }

        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;
        public string DescriptionAr { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }

        public virtual AboutUs AboutUs { get; set; } = default!;
    }
}

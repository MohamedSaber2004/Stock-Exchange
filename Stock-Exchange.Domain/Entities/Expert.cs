using Stock_Exchange.Domain.Common;

namespace Stock_Exchange.Domain.Entities
{
    public class Expert : BaseEntity<Guid>
    {
        public string FullNameEn { get; set; } = string.Empty;
        public string FullNameAr { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsFeaturedOnHome { get; set; } = true;
    }
}

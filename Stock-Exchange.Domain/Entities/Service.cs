using Stock_Exchange.Domain.Common;

namespace Stock_Exchange.Domain.Entities
{
    public class Service : BaseEntity<Guid>
    {
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;
        public string DescriptionAr { get; set; } = string.Empty;
        public string ContentEn { get; set; } = string.Empty;
        public string ContentAr { get; set; } = string.Empty;
        public string IconName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? LinkRoute { get; set; }
        public int DisplayOrder { get; set; }
    }
}

using Stock_Exchange.Domain.Common;

namespace Stock_Exchange.Domain.Entities
{
    public class ExpertConsultationArea : BaseEntity<Guid>
    {
        public Guid ExpertId { get; set; }
        public string TextEn { get; set; } = string.Empty;
        public string TextAr { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }

        public virtual Expert Expert { get; set; } = default!;
    }
}

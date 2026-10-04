using Stock_Exchange.Domain.Common;

namespace Stock_Exchange.Domain.Entities
{
    public class VideoCategory : BaseEntity<Guid>
    {
        public string CategoryArName { get; set; } = string.Empty;
        public string CategoryEnName { get; set; } = string.Empty;

        public virtual ICollection<Video> Videos { get; set; } = new List<Video>();
    }
}

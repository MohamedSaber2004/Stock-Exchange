using Stock_Exchange.Domain.Common;

namespace Stock_Exchange.Domain.Entities
{
    public class ArticleCategory : BaseEntity<Guid>
    {
        public string CategoryArName { get; set; } = string.Empty;
        public string CategoryEnName { get; set; } = string.Empty;

        public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
    }
}

using Stock_Exchange.Domain.Common;

namespace Stock_Exchange.Domain.Entities
{
    public class Country : BaseEntity<Guid>
    {
        public string CountryArName { get; set; } = string.Empty;
        public string CountryEnName { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;

        public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }
}

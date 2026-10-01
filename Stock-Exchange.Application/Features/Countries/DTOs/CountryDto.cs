namespace Stock_Exchange.Application.Features.Countries.DTOs
{
    public class CountryDto
    {
        public Guid Id { get; set; }
        public string CountryArName { get; set; } = string.Empty;
        public string CountryEnName { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public int UsersCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

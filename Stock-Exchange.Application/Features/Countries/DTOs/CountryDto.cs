namespace Stock_Exchange.Application.Features.Countries.DTOs
{
    public class CountryDto
    {
        public Guid Id { get; set; }
        public string CountryArName { get; set; } = string.Empty;
        public string CountryEnName { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}

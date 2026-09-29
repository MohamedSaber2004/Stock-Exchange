namespace Stock_Exchange.Application.Features.AboutUs.DTOs
{
    public class AboutUsFeatureRequest
    {
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;
        public string DescriptionAr { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
}

namespace Stock_Exchange.Application.Features.AboutUs.DTOs
{
    public class AboutUsDto
    {
        public Guid Id { get; set; }
        public string StoryEn { get; set; } = string.Empty;
        public string StoryAr { get; set; } = string.Empty;
        public string MissionEn { get; set; } = string.Empty;
        public string MissionAr { get; set; } = string.Empty;
        public string VisionEn { get; set; } = string.Empty;
        public string VisionAr { get; set; } = string.Empty;
        public string? SupportEmail { get; set; }
        public List<AboutUsFeatureDto> Features { get; set; } = new();
    }
}

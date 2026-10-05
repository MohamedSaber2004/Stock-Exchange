using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.Services.DTOs
{
    public class ServiceDto
    {
        public Guid Id { get; set; }
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
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public string Title => !string.IsNullOrEmpty(TitleEn) ? TitleEn : TitleAr;
        public string Description => !string.IsNullOrEmpty(DescriptionEn) ? DescriptionEn : DescriptionAr;
        public string Content => !string.IsNullOrEmpty(ContentEn) ? ContentEn : ContentAr;

        public void ApplyLanguageFilter(Language language)
        {
            if (language == Language.en)
            {
                TitleAr = string.Empty;
                DescriptionAr = string.Empty;
                ContentAr = string.Empty;
                return;
            }

            TitleEn = string.Empty;
            DescriptionEn = string.Empty;
            ContentEn = string.Empty;
        }
    }
}

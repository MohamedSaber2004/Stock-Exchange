using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.Videos.DTOs
{
    public class VideoDto
    {
        public Guid Id { get; set; }
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public string? VideoUrl { get; set; }
        public int DurationSeconds { get; set; }
        public string InstructorName { get; set; } = string.Empty;
        public string CategoryEn { get; set; } = string.Empty;
        public string CategoryAr { get; set; } = string.Empty;
        public Guid? CategoryId { get; set; }
        public Guid? VideoCategoryId { get; set; }
        public string? CategoryEnName { get; set; }
        public string? CategoryArName { get; set; }
        public bool IsPreviewable { get; set; }
        public bool IsFeaturedOnHome { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public string Title => !string.IsNullOrEmpty(TitleEn) ? TitleEn : TitleAr;
        public string Category => !string.IsNullOrEmpty(CategoryEnName) ? CategoryEnName : (!string.IsNullOrEmpty(CategoryEn) ? CategoryEn : CategoryAr);

        public void ApplyLanguageFilter(Language language)
        {
            if (language == Language.en)
            {
                TitleAr = string.Empty;
                CategoryAr = string.Empty;
                CategoryArName = null;
                return;
            }

            TitleEn = string.Empty;
            CategoryEn = string.Empty;
            CategoryEnName = null;
        }
    }
}

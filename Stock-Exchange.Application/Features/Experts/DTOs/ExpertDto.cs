using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.Experts.DTOs
{
    public class ExpertDto
    {
        public Guid Id { get; set; }
        public string FullNameEn { get; set; } = string.Empty;
        public string FullNameAr { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsFeaturedOnHome { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public string FullName => !string.IsNullOrEmpty(FullNameEn) ? FullNameEn : FullNameAr;
        public string Title => !string.IsNullOrEmpty(TitleEn) ? TitleEn : TitleAr;

        public void ApplyLanguageFilter(Language language)
        {
            if (language == Language.en)
            {
                FullNameAr = string.Empty;
                TitleAr = string.Empty;
                return;
            }

            FullNameEn = string.Empty;
            TitleEn = string.Empty;
        }
    }
}

using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.Home.DTOs
{
    public class HomePlanFeatureDto
    {
        public Guid Id { get; set; }
        public string TextEn { get; set; } = string.Empty;
        public string TextAr { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }

        public string Text => !string.IsNullOrEmpty(TextEn) ? TextEn : TextAr;

        public void ApplyLanguageFilter(Language language)
        {
            if (language == Language.en)
            {
                TextAr = string.Empty;
                return;
            }

            TextEn = string.Empty;
        }
    }

    public class HomePlanDto
    {
        public Guid Id { get; set; }
        public string NameEn { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public decimal PriceEgp { get; set; }
        public string Period { get; set; } = "mo";
        public bool IsHighlighted { get; set; }
        public int DisplayOrder { get; set; }
        public List<string> Features { get; set; } = new();
        public List<HomePlanFeatureDto> FeatureItems { get; set; } = new();

        public string Name => !string.IsNullOrEmpty(NameEn) ? NameEn : NameAr;

        public void ApplyLanguageFilter(Language language)
        {
            if (language == Language.en)
            {
                NameAr = string.Empty;
                foreach (var item in FeatureItems)
                {
                    item.ApplyLanguageFilter(language);
                }
                Features = FeatureItems.Select(f => f.TextEn).Where(t => !string.IsNullOrWhiteSpace(t)).ToList();
                return;
            }

            NameEn = string.Empty;
            foreach (var item in FeatureItems)
            {
                item.ApplyLanguageFilter(language);
            }
            Features = FeatureItems.Select(f => f.TextAr).Where(t => !string.IsNullOrWhiteSpace(t)).ToList();
        }
    }
}

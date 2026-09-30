using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.Home.DTOs
{
    public class HomeNewsItemRequest
    {
        public Guid? Id { get; set; }
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string SummaryEn { get; set; } = string.Empty;
        public string SummaryAr { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string CategoryEn { get; set; } = string.Empty;
        public string CategoryAr { get; set; } = string.Empty;
        public DateTime? PublishedAt { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsFeaturedOnHome { get; set; } = true;
    }

    public class HomeServiceItemRequest
    {
        public Guid? Id { get; set; }
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;
        public string DescriptionAr { get; set; } = string.Empty;
        public string? IconName { get; set; }
        public string? ImageUrl { get; set; }
        public string? LinkRoute { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class HomeArticleItemRequest
    {
        public Guid? Id { get; set; }
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string ExcerptEn { get; set; } = string.Empty;
        public string ExcerptAr { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public int ReadMinutes { get; set; }
        public DateTime? PublishedAt { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsFeaturedOnHome { get; set; } = true;
    }

    public class HomeVideoItemRequest
    {
        public Guid? Id { get; set; }
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public string? VideoUrl { get; set; }
        public string InstructorName { get; set; } = string.Empty;
        public string CategoryEn { get; set; } = string.Empty;
        public string CategoryAr { get; set; } = string.Empty;
        public bool IsPreviewable { get; set; } = true;
        public bool IsFeaturedOnHome { get; set; } = true;
        public int DisplayOrder { get; set; }
    }

    public class HomePlanFeatureItemRequest
    {
        public Guid? Id { get; set; }
        public string TextEn { get; set; } = string.Empty;
        public string TextAr { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }

    public class HomePlanItemRequest
    {
        public Guid? Id { get; set; }
        public string NameEn { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public decimal PriceEgp { get; set; }
        public SubscriptionPeriod Period { get; set; } = SubscriptionPeriod.Monthly;
        public bool IsHighlighted { get; set; }
        public int DisplayOrder { get; set; }
        public List<HomePlanFeatureItemRequest> Features { get; set; } = new();
    }

    public class HomeExpertItemRequest
    {
        public Guid? Id { get; set; }
        public string FullNameEn { get; set; } = string.Empty;
        public string FullNameAr { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsFeaturedOnHome { get; set; } = true;
    }
}

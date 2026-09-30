using Stock_Exchange.Application.Common.Helpers;
using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.ActivityLogs.DTOs
{
    public class ActivityLogDto
    {
        public Guid Id { get; set; }
        public string FormattedId { get; set; } = string.Empty;
        public Guid? UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string? UserProfilePictureUrl { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? ActionAr { get; set; }
        public string? ActionEn { get; set; }
        public ActivityResourceType ResourceType { get; set; }
        public string ResourceTypeArabic => GetResourceTypeArabic(ResourceType);
        public string ResourceTypeEnglish => GetResourceTypeEnglish(ResourceType);
        public string ResourceTypeName => ResourceType.ToString();
        public string ResourceTypeTitle { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string Device { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string TimeAgo { get; set; } = string.Empty;
        public string TimeAgoArabic => TimeAgoHelper.ToArabicTimeAgo(CreatedAt);
        public string TimeAgoEnglish => TimeAgoHelper.ToEnglishTimeAgo(CreatedAt);
        public string? Details { get; set; }
        public string? DetailsAr { get; set; }
        public string? DetailsEn { get; set; }

        public void ApplyLanguageFilter(Language language)
        {
            if (language == Language.ar)
            {
                Action = !string.IsNullOrEmpty(ActionAr) ? ActionAr : Action;
                Details = !string.IsNullOrEmpty(DetailsAr) ? DetailsAr : Details;
                ResourceTypeTitle = ResourceTypeArabic;
                TimeAgo = TimeAgoArabic;
            }
            else
            {
                Action = !string.IsNullOrEmpty(ActionEn) ? ActionEn : Action;
                Details = !string.IsNullOrEmpty(DetailsEn) ? DetailsEn : Details;
                ResourceTypeTitle = ResourceTypeEnglish;
                TimeAgo = TimeAgoEnglish;
            }
        }

        public static string GetResourceTypeArabic(ActivityResourceType type) => type switch
        {
            ActivityResourceType.UserRegistrations => "تسجيلات المستخدمين",
            ActivityResourceType.Users => "المستخدمون",
            ActivityResourceType.Articles => "المقالات",
            ActivityResourceType.Videos => "الفيديوهات",
            ActivityResourceType.News => "الأخبار",
            ActivityResourceType.HelpCenter => "مركز المساعدة",
            ActivityResourceType.SubscriptionPlans => "خطط الاشتراكات",
            ActivityResourceType.AboutUs => "من نحن",
            ActivityResourceType.TermsAndConditions => "الشروط والأحكام",
            ActivityResourceType.PrivacyPolicy => "سياسة الخصوصية",
            _ => "النظام"
        };

        public static string GetResourceTypeEnglish(ActivityResourceType type) => type switch
        {
            ActivityResourceType.UserRegistrations => "User Registrations",
            ActivityResourceType.Users => "Users",
            ActivityResourceType.Articles => "Articles",
            ActivityResourceType.Videos => "Videos",
            ActivityResourceType.News => "News",
            ActivityResourceType.HelpCenter => "Help Center",
            ActivityResourceType.SubscriptionPlans => "Subscription Plans",
            ActivityResourceType.AboutUs => "About Us",
            ActivityResourceType.TermsAndConditions => "Terms & Conditions",
            ActivityResourceType.PrivacyPolicy => "Privacy Policy",
            _ => "System"
        };
    }
}

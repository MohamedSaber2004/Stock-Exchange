using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.ActivityLogs.DTOs
{
    public class ActivityLogsSummaryDto
    {
        public int TotalLogs { get; set; }
        public int UserRegistrationsCount { get; set; }
        public int ContentOperationsCount { get; set; }
        public int ArticlesCount { get; set; }
        public int VideosCount { get; set; }
        public int UsersCount { get; set; }

        public List<SummaryCardDto> Cards { get; set; } = new();

        public void ApplyLanguageFilter(Language language)
        {
            foreach (var card in Cards)
            {
                card.ApplyLanguageFilter(language);
            }
        }

        public static ActivityLogsSummaryDto Create(
            int total,
            int userRegs,
            int articles,
            int videos,
            int contentOps,
            int users,
            Language language)
        {
            var summary = new ActivityLogsSummaryDto
            {
                TotalLogs = total,
                UserRegistrationsCount = userRegs,
                ArticlesCount = articles,
                VideosCount = videos,
                ContentOperationsCount = contentOps,
                UsersCount = users,
                Cards = new List<SummaryCardDto>
                {
                    new SummaryCardDto
                    {
                        Key = "total",
                        TitleAr = "إجمالي السجلات",
                        TitleEn = "Total Logs",
                        Count = total
                    },
                    new SummaryCardDto
                    {
                        Key = "registrations",
                        TitleAr = "تسجيلات المستخدمين",
                        TitleEn = "User Registrations",
                        Count = userRegs
                    },
                    new SummaryCardDto
                    {
                        Key = "content",
                        TitleAr = "عمليات المحتوى",
                        TitleEn = "Content Operations",
                        Count = contentOps
                    },
                    new SummaryCardDto
                    {
                        Key = "articles",
                        TitleAr = "المقالات",
                        TitleEn = "Articles",
                        Count = articles
                    },
                    new SummaryCardDto
                    {
                        Key = "videos",
                        TitleAr = "الفيديوهات",
                        TitleEn = "Videos",
                        Count = videos
                    },
                    new SummaryCardDto
                    {
                        Key = "users",
                        TitleAr = "إدارة المستخدمين",
                        TitleEn = "User Management",
                        Count = users
                    }
                }
            };

            summary.ApplyLanguageFilter(language);
            return summary;
        }
    }

    public class SummaryCardDto
    {
        public string Key { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int Count { get; set; }

        public void ApplyLanguageFilter(Language language)
        {
            Title = language == Language.ar ? TitleAr : TitleEn;
        }
    }
}

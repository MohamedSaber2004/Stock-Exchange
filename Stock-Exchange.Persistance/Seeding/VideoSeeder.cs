using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Persistance.Seeding
{
    public static class VideoSeeder
    {
        public static async Task SeedVideosAsync(StockExchangeDbContext context)
        {
            // Remove old placeholder video if present
            var placeholder = await context.Videos.IgnoreQueryFilters()
                .FirstOrDefaultAsync(v => v.TitleEn == "Investing 101: Where to Start");
            if (placeholder is not null)
            {
                context.Videos.Remove(placeholder);
                await context.SaveChangesAsync();
            }

            var existingTitles = await context.Videos.IgnoreQueryFilters()
                .Select(v => v.TitleEn)
                .ToListAsync();

            var videos = new List<Video>
            {
                new Video
                {
                    TitleEn = "Introduction to Stock Market",
                    TitleAr = "مقدمة إلى سوق الأسهم",
                    ThumbnailUrl = null,
                    VideoUrl = "https://www.youtube.com/watch?v=example1",
                    DurationSeconds = 720,
                    InstructorName = "Ahmed Al-Rashidi",
                    CategoryEn = "Basics",
                    CategoryAr = "الأساسيات",
                    IsPreviewable = true,
                    IsFeaturedOnHome = true,
                    DisplayOrder = 1
                },
                new Video
                {
                    TitleEn = "Technical Analysis Fundamentals",
                    TitleAr = "أساسيات التحليل الفني",
                    ThumbnailUrl = null,
                    VideoUrl = "https://www.youtube.com/watch?v=example2",
                    DurationSeconds = 1080,
                    InstructorName = "Sara Al-Mansouri",
                    CategoryEn = "Technical Analysis",
                    CategoryAr = "التحليل الفني",
                    IsPreviewable = true,
                    IsFeaturedOnHome = true,
                    DisplayOrder = 2
                },
                new Video
                {
                    TitleEn = "Understanding Candlestick Charts",
                    TitleAr = "فهم مخططات الشموع اليابانية",
                    ThumbnailUrl = null,
                    VideoUrl = "https://www.youtube.com/watch?v=example3",
                    DurationSeconds = 900,
                    InstructorName = "Khalid Al-Otaibi",
                    CategoryEn = "Technical Analysis",
                    CategoryAr = "التحليل الفني",
                    IsPreviewable = false,
                    IsFeaturedOnHome = true,
                    DisplayOrder = 3
                },
                new Video
                {
                    TitleEn = "Risk Management in Trading",
                    TitleAr = "إدارة المخاطر في التداول",
                    ThumbnailUrl = null,
                    VideoUrl = "https://www.youtube.com/watch?v=example4",
                    DurationSeconds = 1200,
                    InstructorName = "Mohammed Al-Zahrani",
                    CategoryEn = "Risk Management",
                    CategoryAr = "إدارة المخاطر",
                    IsPreviewable = true,
                    IsFeaturedOnHome = false,
                    DisplayOrder = 4
                },
                new Video
                {
                    TitleEn = "Fundamental Analysis Explained",
                    TitleAr = "شرح التحليل الأساسي",
                    ThumbnailUrl = null,
                    VideoUrl = "https://www.youtube.com/watch?v=example5",
                    DurationSeconds = 1500,
                    InstructorName = "Fatima Al-Harbi",
                    CategoryEn = "Fundamental Analysis",
                    CategoryAr = "التحليل الأساسي",
                    IsPreviewable = false,
                    IsFeaturedOnHome = true,
                    DisplayOrder = 5
                },
                new Video
                {
                    TitleEn = "Building a Profitable Portfolio",
                    TitleAr = "بناء محفظة استثمارية مربحة",
                    ThumbnailUrl = null,
                    VideoUrl = "https://www.youtube.com/watch?v=example6",
                    DurationSeconds = 1800,
                    InstructorName = "Omar Al-Ghamdi",
                    CategoryEn = "Portfolio Management",
                    CategoryAr = "إدارة المحافظ",
                    IsPreviewable = false,
                    IsFeaturedOnHome = false,
                    DisplayOrder = 6
                }
            };

            var toAdd = videos.Where(v => !existingTitles.Contains(v.TitleEn)).ToList();
            if (toAdd.Count > 0)
            {
                await context.Videos.AddRangeAsync(toAdd);
                await context.SaveChangesAsync();
            }
        }
    }
}

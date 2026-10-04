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

                        // Clean up any existing static/external links
            var staticVideos = await context.Videos.IgnoreQueryFilters()
                .Where(v => v.VideoUrl != null && (v.VideoUrl.StartsWith("http://") || v.VideoUrl.StartsWith("https://")))
                .ToListAsync();
            if (staticVideos.Count > 0)
            {
                foreach (var sv in staticVideos)
                {
                    sv.VideoUrl = null;
                }
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
                    TitleAr = "Ù…Ù‚Ø¯Ù…Ø© Ø¥Ù„Ù‰ Ø³ÙˆÙ‚ Ø§Ù„Ø£Ø³Ù‡Ù…",
                    ThumbnailUrl = null,
                    VideoUrl = null,
                    DurationSeconds = 720,
                    InstructorName = "Ahmed Al-Rashidi",
                    CategoryEn = "Basics",
                    CategoryAr = "Ø§Ù„Ø£Ø³Ø§Ø³ÙŠØ§Øª",
                    IsPreviewable = true,
                    IsFeaturedOnHome = true,
                    DisplayOrder = 1
                },
                new Video
                {
                    TitleEn = "Technical Analysis Fundamentals",
                    TitleAr = "Ø£Ø³Ø§Ø³ÙŠØ§Øª Ø§Ù„ØªØ­Ù„ÙŠÙ„ Ø§Ù„ÙÙ†ÙŠ",
                    ThumbnailUrl = null,
                    VideoUrl = null,
                    DurationSeconds = 1080,
                    InstructorName = "Sara Al-Mansouri",
                    CategoryEn = "Technical Analysis",
                    CategoryAr = "Ø§Ù„ØªØ­Ù„ÙŠÙ„ Ø§Ù„ÙÙ†ÙŠ",
                    IsPreviewable = true,
                    IsFeaturedOnHome = true,
                    DisplayOrder = 2
                },
                new Video
                {
                    TitleEn = "Understanding Candlestick Charts",
                    TitleAr = "ÙÙ‡Ù… Ù…Ø®Ø·Ø·Ø§Øª Ø§Ù„Ø´Ù…ÙˆØ¹ Ø§Ù„ÙŠØ§Ø¨Ø§Ù†ÙŠØ©",
                    ThumbnailUrl = null,
                    VideoUrl = null,
                    DurationSeconds = 900,
                    InstructorName = "Khalid Al-Otaibi",
                    CategoryEn = "Technical Analysis",
                    CategoryAr = "Ø§Ù„ØªØ­Ù„ÙŠÙ„ Ø§Ù„ÙÙ†ÙŠ",
                    IsPreviewable = false,
                    IsFeaturedOnHome = true,
                    DisplayOrder = 3
                },
                new Video
                {
                    TitleEn = "Risk Management in Trading",
                    TitleAr = "Ø¥Ø¯Ø§Ø±Ø© Ø§Ù„Ù…Ø®Ø§Ø·Ø± ÙÙŠ Ø§Ù„ØªØ¯Ø§ÙˆÙ„",
                    ThumbnailUrl = null,
                    VideoUrl = null,
                    DurationSeconds = 1200,
                    InstructorName = "Mohammed Al-Zahrani",
                    CategoryEn = "Risk Management",
                    CategoryAr = "Ø¥Ø¯Ø§Ø±Ø© Ø§Ù„Ù…Ø®Ø§Ø·Ø±",
                    IsPreviewable = true,
                    IsFeaturedOnHome = false,
                    DisplayOrder = 4
                },
                new Video
                {
                    TitleEn = "Fundamental Analysis Explained",
                    TitleAr = "Ø´Ø±Ø­ Ø§Ù„ØªØ­Ù„ÙŠÙ„ Ø§Ù„Ø£Ø³Ø§Ø³ÙŠ",
                    ThumbnailUrl = null,
                    VideoUrl = null,
                    DurationSeconds = 1500,
                    InstructorName = "Fatima Al-Harbi",
                    CategoryEn = "Fundamental Analysis",
                    CategoryAr = "Ø§Ù„ØªØ­Ù„ÙŠÙ„ Ø§Ù„Ø£Ø³Ø§Ø³ÙŠ",
                    IsPreviewable = false,
                    IsFeaturedOnHome = true,
                    DisplayOrder = 5
                },
                new Video
                {
                    TitleEn = "Building a Profitable Portfolio",
                    TitleAr = "Ø¨Ù†Ø§Ø¡ Ù…Ø­ÙØ¸Ø© Ø§Ø³ØªØ«Ù…Ø§Ø±ÙŠØ© Ù…Ø±Ø¨Ø­Ø©",
                    ThumbnailUrl = null,
                    VideoUrl = null,
                    DurationSeconds = 1800,
                    InstructorName = "Omar Al-Ghamdi",
                    CategoryEn = "Portfolio Management",
                    CategoryAr = "Ø¥Ø¯Ø§Ø±Ø© Ø§Ù„Ù…Ø­Ø§ÙØ¸",
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

using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Persistance.Seeding
{
    public static class ArticleSeeder
    {
        public static async Task SeedArticlesAsync(StockExchangeDbContext context)
        {
            // Remove old placeholder article if present
            var placeholder = await context.Articles.IgnoreQueryFilters()
                .FirstOrDefaultAsync(a => a.TitleEn == "The Beginner's Guide to Building Wealth");
            if (placeholder is not null)
            {
                context.Articles.Remove(placeholder);
                await context.SaveChangesAsync();
            }

            var existingTitles = await context.Articles.IgnoreQueryFilters()
                .Select(a => a.TitleEn)
                .ToListAsync();

            var articles = new List<Article>
            {
                new Article
                {
                    TitleEn = "How to Start Investing in the Stock Market",
                    TitleAr = "كيف تبدأ الاستثمار في سوق الأسهم",
                    ExcerptEn = "A beginner's guide to understanding how the stock market works and how to make your first investment.",
                    ExcerptAr = "دليل المبتدئين لفهم كيفية عمل سوق الأسهم وكيفية إجراء استثمارك الأول.",
                    ImageUrl = null,
                    AuthorName = "Ahmed Al-Rashidi",
                    ReadMinutes = 5,
                    PublishedAt = DateTime.UtcNow.AddDays(-30),
                    IsFeaturedOnHome = true,
                    DisplayOrder = 1
                },
                new Article
                {
                    TitleEn = "Top 5 Technical Indicators Every Trader Should Know",
                    TitleAr = "أفضل 5 مؤشرات فنية يجب أن يعرفها كل متداول",
                    ExcerptEn = "Discover the most powerful technical indicators used by professional traders to predict market movements.",
                    ExcerptAr = "اكتشف أقوى المؤشرات الفنية التي يستخدمها المتداولون المحترفون للتنبؤ بتحركات السوق.",
                    ImageUrl = null,
                    AuthorName = "Sara Al-Mansouri",
                    ReadMinutes = 7,
                    PublishedAt = DateTime.UtcNow.AddDays(-25),
                    IsFeaturedOnHome = true,
                    DisplayOrder = 2
                },
                new Article
                {
                    TitleEn = "Understanding Market Volatility and How to Manage It",
                    TitleAr = "فهم تقلبات السوق وكيفية إدارتها",
                    ExcerptEn = "Market volatility can be intimidating, but with the right strategies, it can also be a source of opportunity.",
                    ExcerptAr = "قد تكون تقلبات السوق مخيفة، ولكن مع الاستراتيجيات الصحيحة يمكن أن تكون مصدر فرصة.",
                    ImageUrl = null,
                    AuthorName = "Khalid Al-Otaibi",
                    ReadMinutes = 6,
                    PublishedAt = DateTime.UtcNow.AddDays(-20),
                    IsFeaturedOnHome = true,
                    DisplayOrder = 3
                },
                new Article
                {
                    TitleEn = "Dividend Investing: A Strategy for Passive Income",
                    TitleAr = "الاستثمار في الأرباح الموزعة: استراتيجية للدخل السلبي",
                    ExcerptEn = "Learn how dividend investing can provide a steady stream of passive income while growing your portfolio.",
                    ExcerptAr = "تعرف على كيفية توفير الاستثمار في الأرباح الموزعة لتدفق مستمر من الدخل السلبي مع نمو محفظتك.",
                    ImageUrl = null,
                    AuthorName = "Mohammed Al-Zahrani",
                    ReadMinutes = 8,
                    PublishedAt = DateTime.UtcNow.AddDays(-15),
                    IsFeaturedOnHome = false,
                    DisplayOrder = 4
                },
                new Article
                {
                    TitleEn = "The Psychology of Trading: Controlling Emotions",
                    TitleAr = "سيكولوجية التداول: التحكم في المشاعر",
                    ExcerptEn = "Emotional discipline is one of the most critical skills for successful trading. Here's how to master it.",
                    ExcerptAr = "الانضباط العاطفي هو أحد أهم المهارات للتداول الناجح. إليك كيفية إتقانها.",
                    ImageUrl = null,
                    AuthorName = "Fatima Al-Harbi",
                    ReadMinutes = 6,
                    PublishedAt = DateTime.UtcNow.AddDays(-10),
                    IsFeaturedOnHome = true,
                    DisplayOrder = 5
                },
                new Article
                {
                    TitleEn = "Global Market Trends to Watch in 2025",
                    TitleAr = "اتجاهات السوق العالمية التي يجب مراقبتها في 2025",
                    ExcerptEn = "An in-depth look at the key trends shaping global financial markets and what they mean for investors.",
                    ExcerptAr = "نظرة معمقة على الاتجاهات الرئيسية التي تشكل الأسواق المالية العالمية وما تعنيه للمستثمرين.",
                    ImageUrl = null,
                    AuthorName = "Omar Al-Ghamdi",
                    ReadMinutes = 10,
                    PublishedAt = DateTime.UtcNow.AddDays(-5),
                    IsFeaturedOnHome = false,
                    DisplayOrder = 6
                }
            };

            var toAdd = articles.Where(a => !existingTitles.Contains(a.TitleEn)).ToList();
            if (toAdd.Count > 0)
            {
                await context.Articles.AddRangeAsync(toAdd);
                await context.SaveChangesAsync();
            }
        }
    }
}

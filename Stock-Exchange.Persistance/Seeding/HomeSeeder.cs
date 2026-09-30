using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Persistance.Seeding
{
    public static class HomeSeeder
    {
        public static async Task SeedHomeAsync(StockExchangeDbContext context)
        {
            var existingHome = await context.Homes.FirstOrDefaultAsync();
            if (existingHome is null)
            {
                var home = new Home
                {
                    HeroTitleEn = "Learn. Analyze. Invest Smarter.",
                    HeroTitleAr = "تعلّم. حلّل. استثمر بذكاء.",
                    HeroSubtitleEn = "Your all-in-one platform for financial education, market insights, and expert analysis.",
                    HeroSubtitleAr = "منصتك المتكاملة للتعليم المالي، ورؤى السوق، والتحليلات المتخصصة.",
                    HeroImageUrl = null
                };

                await context.Homes.AddAsync(home);
            }

            if (!await context.Services.AnyAsync())
            {
                var services = new List<Service>
                {
                    new()
                    {
                        TitleEn = "Market News",
                        TitleAr = "أخبار السوق",
                        DescriptionEn = "Real-time headlines & breaking market alerts",
                        DescriptionAr = "أحدث العناوين وتنبيهات السوق الفورية",
                        IconName = "newspaper",
                        DisplayOrder = 1
                    },
                    new()
                    {
                        TitleEn = "Portfolio Tracker",
                        TitleAr = "محفظة الاستثمار",
                        DescriptionEn = "Track and analyze your assets seamlessly",
                        DescriptionAr = "تتبع وتحليل أصولك الاستثمارية بسهولة",
                        IconName = "briefcase",
                        DisplayOrder = 2
                    },
                    new()
                    {
                        TitleEn = "Subscriptions",
                        TitleAr = "الاشتراكات",
                        DescriptionEn = "Unlock premium insights & expert courses",
                        DescriptionAr = "احصل على تحليلات متميزة ودورات متقدمة",
                        IconName = "credit-card",
                        DisplayOrder = 3
                    }
                };

                await context.Services.AddRangeAsync(services);
            }

            // ── 3. Subscription Plans ──
            if (!await context.SubscriptionPlans.AnyAsync())
            {
                var freePlan = new SubscriptionPlan
                {
                    NameEn = "Free",
                    NameAr = "مجاني",
                    PriceEgp = 0m,
                    Period = Domain.Enums.SubscriptionPeriod.Monthly,
                    IsHighlighted = false,
                    DisplayOrder = 1,
                    Features = new List<PlanFeature>
                    {
                        new() { TextEn = "Daily market news", TextAr = "أخبار السوق اليومية", DisplayOrder = 1 },
                        new() { TextEn = "Limited articles access", TextAr = "وصول محدود للمقالات", DisplayOrder = 2 }
                    }
                };

                var basicPlan = new SubscriptionPlan
                {
                    NameEn = "Basic",
                    NameAr = "أساسي",
                    PriceEgp = 200m,
                    Period = Domain.Enums.SubscriptionPeriod.Monthly,
                    IsHighlighted = true,
                    DisplayOrder = 2,
                    Features = new List<PlanFeature>
                    {
                        new() { TextEn = "All market news & instant alerts", TextAr = "جميع الأخبار وتنبيهات فورية", DisplayOrder = 1 },
                        new() { TextEn = "Full articles & research reports", TextAr = "وصول كامل للمقالات والتقارير", DisplayOrder = 2 },
                        new() { TextEn = "Standard video courses & previews", TextAr = "دورات تدريبية وفيديوهات تمهيدية", DisplayOrder = 3 }
                    }
                };

                await context.SubscriptionPlans.AddRangeAsync(freePlan, basicPlan);
            }

            // ── 4. Initial News ──
            if (!await context.News.AnyAsync())
            {
                var newsList = new List<News>
                {
                    new()
                    {
                        TitleEn = "Global markets rally as inflation shows signs of cooling",
                        TitleAr = "ارتفاع الأسواق العالمية مع بوادر تراجع التضخم",
                        SummaryEn = "Major global stock indices experienced solid gains today following positive economic reports.",
                        SummaryAr = "حققت مؤشرات الأسهم العالمية الرئيسية مكاسب قوية اليوم في أعقاب تقارير اقتصادية إيجابية.",
                        CategoryEn = "Markets",
                        CategoryAr = "أسواق",
                        PublishedAt = DateTime.UtcNow.AddHours(-2),
                        DisplayOrder = 1,
                        IsFeaturedOnHome = true
                    },
                    new()
                    {
                        TitleEn = "Central banks signal steady interest rates for next quarter",
                        TitleAr = "البنوك المركزية تشير إلى استقرار أسعار الفائدة للربع القادم",
                        SummaryEn = "Monetary policy committees maintain a balanced stance amidst stable economic growth.",
                        SummaryAr = "لجان السياسة النقدية تحافظ على موقف متوازن في ظل نمو اقتصادي مستقر.",
                        CategoryEn = "Economy",
                        CategoryAr = "اقتصاد",
                        PublishedAt = DateTime.UtcNow.AddHours(-6),
                        DisplayOrder = 2,
                        IsFeaturedOnHome = true
                    }
                };

                await context.News.AddRangeAsync(newsList);
            }

            // ── 5. Initial Articles ──
            if (!await context.Articles.AnyAsync())
            {
                var articles = new List<Article>
                {
                    new()
                    {
                        TitleEn = "The Beginner's Guide to Building Wealth",
                        TitleAr = "دليل المبتدئين لبناء الثروة والاستثمار",
                        ExcerptEn = "Learn the foundational principles of compounding, diversification, and disciplined saving.",
                        ExcerptAr = "تعرّف على المبادئ الأساسية للعائد التراكمي، وتنويع المحفظة، والادخار المنضبط.",
                        AuthorName = "Emily Carter",
                        ReadMinutes = 5,
                        PublishedAt = DateTime.UtcNow.AddDays(-1),
                        DisplayOrder = 1,
                        IsFeaturedOnHome = true
                    }
                };

                await context.Articles.AddRangeAsync(articles);
            }

            // ── 6. Initial Videos ──
            if (!await context.Videos.AnyAsync())
            {
                var videos = new List<Video>
                {
                    new()
                    {
                        TitleEn = "Investing 101: Where to Start",
                        TitleAr = "أساسيات الاستثمار 101: من أين تبدأ",
                        DurationSeconds = 750, // 12:30
                        InstructorName = "Marcus Ali",
                        CategoryEn = "Technical Analysis",
                        CategoryAr = "التحليل الفني",
                        IsPreviewable = true,
                        IsFeaturedOnHome = true,
                        DisplayOrder = 1
                    }
                };

                await context.Videos.AddRangeAsync(videos);
            }

            // ── 7. Initial Experts ──
            if (!await context.Experts.AnyAsync())
            {
                var experts = new List<Expert>
                {
                    new()
                    {
                        FullNameEn = "Michael Reed",
                        FullNameAr = "مايكل ريد",
                        TitleEn = "Equity Markets",
                        TitleAr = "أسواق الأسهم",
                        DisplayOrder = 1,
                        IsFeaturedOnHome = true
                    },
                    new()
                    {
                        FullNameEn = "Sarah Jenkins",
                        FullNameAr = "سارة جنكينز",
                        TitleEn = "Commodities & Forex",
                        TitleAr = "السلع والعملات الأجنبية",
                        DisplayOrder = 2,
                        IsFeaturedOnHome = true
                    }
                };

                await context.Experts.AddRangeAsync(experts);
            }

            await context.SaveChangesAsync();
        }
    }
}

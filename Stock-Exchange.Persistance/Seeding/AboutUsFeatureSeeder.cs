using Bogus;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Persistance.Seeding
{
    public static class AboutUsFeatureSeeder
    {
        public static async Task SeedFeaturesAsync(StockExchangeDbContext context)
        {
            if (await context.AboutUsFeatures.AnyAsync())
            {
                return;
            }

            var aboutUs = await context.AboutUs.FirstOrDefaultAsync();
            if (aboutUs is null)
            {
                await AboutUsSeeder.SeedAboutUsAsync(context);
                aboutUs = await context.AboutUs.FirstOrDefaultAsync();
                if (aboutUs is null)
                {
                    return;
                }
            }

            var featuresData = new (string TitleEn, string TitleAr, string DescEn, string DescAr, string Category)[]
            {
                (
                    "Ultra-Fast Execution",
                    "تنفيذ فائق السرعة",
                    "Experience low-latency order execution with state-of-the-art infrastructure designed to handle high transaction volumes smoothly.",
                    "استمتع بتنفيذ الأوامر بأقل زمن استجابة بفضل بنيتنا التحتية المتقدمة المصممة لمعالجة ملايين المعاملات بسلاسة.",
                    "Platform"
                ),
                (
                    "Institutional-Grade Security",
                    "أمان بمستوى المؤسسات الكبرى",
                    "Your assets and personal information are protected with multi-layered encryption, cold wallet storage, and 24/7 fraud monitoring.",
                    "أموالك وبياناتك الشخصية محمية بتشفير متقدم متعدد الطبقات، ومحافظ باردة، ومراقبة احتيال على مدار الساعة.",
                    "Security"
                ),
                (
                    "Advanced Real-Time Analytics",
                    "تحليلات متقدمة في الوقت الفعلي",
                    "Access comprehensive charting tools, technical indicators, and real-time market depth data to make confident investment decisions.",
                    "احصل على أدوات رسوم بيانية شاملة ومؤشرات فنية وبيانات عمق السوق المباشرة لاتخاذ قرارات استثمارية مدروسة.",
                    "Analytics"
                ),
                (
                    "24/7 Dedicated Support",
                    "دعم فني مخصص على مدار الساعة",
                    "Our global customer support team is available around the clock in multiple languages to assist you whenever you need help.",
                    "فريق الدعم الفني العالمي متاح على مدار الساعة بعدة لغات لتقديم المساعدة في أي وقت تحتاجه.",
                    "Support"
                ),
                (
                    "Transparent Competitive Fees",
                    "رسوم تنافسية وشفافة",
                    "Trade with confidence enjoying some of the lowest fees in the industry without any hidden costs or unexpected charges.",
                    "تداول بثقة مع أقل الرسوم التنافسية في السوق المالي دون أي تكاليف خفية أو رسوم غير متوقعة.",
                    "Platform"
                ),
                (
                    "Two-Factor Authentication & Biometrics",
                    "المصادقة الثنائية والتحقق البيومتري",
                    "Enhanced account security featuring hardware keys, authenticator app support, and biometric access verification.",
                    "حماية متقدمة لحسابك عبر مفاتيح الأمان، وتطبيقات المصادقة، والتحقق عبر بصمة الإصبع والوجه.",
                    "Security"
                ),
                (
                    "Regulatory Compliance & Transparency",
                    "الامتثال التنظيمي والشفافية",
                    "Operating in accordance with international financial standards and periodic audits to ensure fund safety and reliability.",
                    "العمل وفق أعلى المعايير التنظيمية المالية الدولية والتدقيق الدوري لضمان سلامة الأموال وموثوقية التعاملات.",
                    "Compliance"
                ),
                (
                    "Multi-Asset Trading Options",
                    "خيارات تداول متعددة الأصول",
                    "Access a diversified portfolio of equities, commodities, and derivatives with seamless currency conversion.",
                    "الوصول إلى محفظة استثمارية متنوعة من الأسهم والسلع والمشتقات المالية مع تحويل سلس بين العملات.",
                    "Trading"
                )
            };

            var features = new List<AboutUsFeature>();
            for (int i = 0; i < featuresData.Length; i++)
            {
                var item = featuresData[i];
                features.Add(new AboutUsFeature
                {
                    AboutUsId = aboutUs.Id,
                    TitleEn = item.TitleEn,
                    TitleAr = item.TitleAr,
                    DescriptionEn = item.DescEn,
                    DescriptionAr = item.DescAr,
                    Category = item.Category,
                    DisplayOrder = i + 1
                });
            }

            await context.AboutUsFeatures.AddRangeAsync(features);
            await context.SaveChangesAsync();
        }
    }
}

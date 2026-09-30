using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Persistance.Seeding
{
    public static class AboutUsSeeder
    {
        public const string DefaultSupportEmail = "support@stockexchange.com";

        public static async Task SeedAboutUsAsync(StockExchangeDbContext context)
        {
            var existing = await context.AboutUs.FirstOrDefaultAsync();

            if (existing is not null)
            {
                if (string.IsNullOrWhiteSpace(existing.SupportEmail))
                {
                    existing.SupportEmail = DefaultSupportEmail;
                    await context.SaveChangesAsync();
                }

                return;
            }

            var aboutUs = new AboutUs
            {
                StoryEn = "Founded with the ambition to democratize financial markets, Stock Exchange provides a state-of-the-art platform offering secure, transparent, and ultra-fast trading solutions for both retail and institutional investors worldwide.",
                StoryAr = "تأسست منصة سوق الأوراق المالية بهدف إتاحة الوصول العادل إلى الأسواق المالية العالمية، وتقديم منصة متطورة وحلول تداول آمنة وشفافة وفائقة السرعة للمستثمرين الأفراد والمؤسسات حول العالم.",
                MissionEn = "To empower individuals and organizations to build wealth and achieve financial independence through innovative technology, reliable market data, and accessible investment opportunities.",
                MissionAr = "تمكين الأفراد والمؤسسات من تنمية ثرواتهم وتحقيق الاستقلال المالي عبر التقنيات المبتكرة وبيانات السوق الموثوقة وفرص الاستثمار المتاحة للجميع.",
                VisionEn = "To become the leading and most trusted global digital financial exchange, setting the highest standards for market integrity, security, and continuous technological innovation.",
                VisionAr = "أن نكون المنصة المالية الرقمية الرائدة والأكثر موثوقية عالمياً، مع وضع أعلى معايير النزاهة والأمان والابتكار التقني المستمر في الأسواق المالية.",
                SupportEmail = DefaultSupportEmail
            };

            await context.AboutUs.AddAsync(aboutUs);
            await context.SaveChangesAsync();
        }
    }
}

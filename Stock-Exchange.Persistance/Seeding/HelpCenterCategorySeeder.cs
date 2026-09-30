using Bogus;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Persistance.Seeding
{
    public static class HelpCenterCategorySeeder
    {
        public static async Task<List<HelpCenterCategory>> SeedCategoriesAsync(StockExchangeDbContext context)
        {
            if (await context.HelpCenterCategories.AnyAsync())
            {
                return await context.HelpCenterCategories.ToListAsync();
            }

            var predefinedCategories = new (string TitleEn, string TitleAr)[]
            {
                ("Account & Verification", "الحساب والتحقق من الهوية"),
                ("Trading & Orders", "التداول وإدارة الأوامر"),
                ("Deposits & Withdrawals", "الإيداع والسحب المالي"),
                ("Security & Two-Factor Authentication", "الأمان والمصادقة الثنائية"),
                ("Fees, Limits & VIP Levels", "الرسوم والحدود ومستويات كبار الشخصيات"),
                ("Mobile & Web Applications", "تطبيقات الهاتف المحمول والويب")
            };

            var categories = new List<HelpCenterCategory>();

            foreach (var item in predefinedCategories)
            {
                categories.Add(new HelpCenterCategory
                {
                    TitleEn = item.TitleEn,
                    TitleAr = item.TitleAr
                });
            }

            await context.HelpCenterCategories.AddRangeAsync(categories);
            await context.SaveChangesAsync();

            return categories;
        }
    }
}

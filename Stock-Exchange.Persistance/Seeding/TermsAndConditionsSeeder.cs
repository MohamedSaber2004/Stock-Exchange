using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Persistance.Seeding
{
    public static class TermsAndConditionsSeeder
    {
        public static async Task SeedTermsAndConditionsAsync(StockExchangeDbContext context)
        {
            if (await context.TermsAndConditions.AnyAsync())
            {
                return;
            }

            var termsAndConditions = new TermsAndConditions
            {
                TitleEn = "Terms & Conditions",
                TitleAr = "الشروط والأحكام",
                DescriptionEn = "Please review the terms and conditions carefully before using our application.",
                DescriptionAr = "يرجى قراءة الشروط والأحكام بعناية قبل استخدام التطبيق الخاص بنا.",
                Sections = new List<TermsAndConditionsSection>
                {
                    new TermsAndConditionsSection
                    {
                        TitleEn = "Acceptance of Terms",
                        TitleAr = "الموافقة على الشروط",
                        ContentEn = "FinWise (\"we\", \"our\", or \"us\") is committed to protecting your personal information and your right to privacy. This Privacy Policy explains what information we collect, how we use it, and what rights you have in relation to it.\n\nBy using FinWise, you agree to the collection and use of information in accordance with this policy. If you do not agree with any part of this policy, please discontinue use of the app.",
                        ContentAr = "تلتزم FinWise (\"نحن\" أو \"الخاصة بنا\") بحماية معلوماتك الشخصية وحقك في الخصوصية. توضح هذه الوثيقة الشروط التي تحكم استخدامك للتطبيق والحقوق والالتزامات المتعلقة به.\n\nباستخدامك لـ FinWise، فإنك تقر وتوافق على الالتزام بجميع بنود هذه الشروط والأحكام. إذا كنت لا توافق على أي جزء منها، يرجى التوقف فوراً عن استخدام التطبيق.",
                        DisplayOrder = 1
                    },
                    new TermsAndConditionsSection
                    {
                        TitleEn = "Account Responsibilities",
                        TitleAr = "مسؤوليات الحساب",
                        ContentEn = "FinWise (\"we\", \"our\", or \"us\") is committed to protecting your personal information and your right to privacy. This Privacy Policy explains what information we collect, how we use it, and what rights you have in relation to it.\n\nBy using FinWise, you agree to the collection and use of information in accordance with this policy. If you do not agree with any part of this policy, please discontinue use of the app.",
                        ContentAr = "أنت مسؤول عن الحفاظ على سرية بيانات تسجيل الدخول الخاصة بحسابك، وعن جميع الأنشطة والمعاملات التي تتم من خلال حسابك. يجب إخطارنا فوراً في حال الاشتباه في أي وصول غير مصرح به.",
                        DisplayOrder = 2
                    },
                    new TermsAndConditionsSection
                    {
                        TitleEn = "Subscriptions & Payments",
                        TitleAr = "الاشتراكات والمدفوعات",
                        ContentEn = "All subscriptions and in-app financial transactions are processed securely. Subscriptions automatically renew at the end of each billing cycle unless cancelled at least 24 hours prior to renewal.",
                        ContentAr = "تتم معالجة جميع الاشتراكات والمدفوعات داخل التطبيق بأمان. تتجدد الاشتراكات تلقائياً في نهاية كل دورة فوترة ما لم يتم الإلغاء قبل 24 ساعة على الأقل من موعد التجديد.",
                        DisplayOrder = 3
                    }
                }
            };

            await context.TermsAndConditions.AddAsync(termsAndConditions);
            await context.SaveChangesAsync();
        }
    }
}

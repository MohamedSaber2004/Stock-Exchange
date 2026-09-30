using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Persistance.Seeding
{
    public static class PrivacyPolicySeeder
    {
        public static async Task SeedPrivacyPolicyAsync(StockExchangeDbContext context)
        {
            if (await context.PrivacyPolicies.AnyAsync())
            {
                return;
            }

            var privacyPolicy = new PrivacyPolicy
            {
                TitleEn = "Privacy Policy",
                TitleAr = "سياسة الخصوصية",
                Sections = new List<PrivacyPolicySection>
                {
                    new PrivacyPolicySection
                    {
                        TitleEn = "Introduction",
                        TitleAr = "المقدمة",
                        ContentEn = "FinWise (\"we\", \"our\", or \"us\") is committed to protecting your personal information and your right to privacy. This Privacy Policy explains what information we collect, how we use it, and what rights you have in relation to it.\n\nBy using FinWise, you agree to the collection and use of information in accordance with this policy. If you do not agree with any part of this policy, please discontinue use of the app.",
                        ContentAr = "تلتزم FinWise (\"نحن\" أو \"الخاصة بنا\") بحماية معلوماتك الشخصية وحقك في الخصوصية. توضح سياسة الخصوصية هذه المعلومات التي نجمعها، وكيف نستخدمها، وما هي حقوقك المتعلقة بها.\n\nباستخدامك لـ FinWise، فإنك توافق على جمع واستخدام المعلومات وفقًا لهذه السياسة. إذا كنت لا توافق على أي جزء من هذه السياسة، يرجى التوقف عن استخدام التطبيق.",
                        DisplayOrder = 1
                    },
                    new PrivacyPolicySection
                    {
                        TitleEn = "Information We Collect",
                        TitleAr = "المعلومات التي نجمعها",
                        ContentEn = "FinWise (\"we\", \"our\", or \"us\") is committed to protecting your personal information and your right to privacy. This Privacy Policy explains what information we collect, how we use it, and what rights you have in relation to it.\n\nBy using FinWise, you agree to the collection and use of information in accordance with this policy. If you do not agree with any part of this policy, please discontinue use of the app.",
                        ContentAr = "تلتزم FinWise (\"نحن\" أو \"الخاصة بنا\") بحماية معلوماتك الشخصية وحقك في الخصوصية. توضح سياسة الخصوصية هذه ما نجمعه من بيانات شخصية وبيانات استخدام، وكيف نستخدم هذه المعلومات، وما هي حقوقك في هذا الشأن.\n\nباستخدامك للتطبيق فإنك توافق على سياسة جمع واستخدام البيانات الموضحة هنا.",
                        DisplayOrder = 2
                    },
                    new PrivacyPolicySection
                    {
                        TitleEn = "How We Use Your Information",
                        TitleAr = "كيف نستخدم معلوماتك",
                        ContentEn = "We use the information we collect to provide, maintain, and improve our services, develop new features, and protect FinWise and our users from fraud and security threats.",
                        ContentAr = "نستخدم المعلومات التي نجمعها لتقديم خدماتنا وصيانتها وتحسينها، وتطوير ميزات وخدمات جديدة، وحماية أمن النظام والمستخدمين من أي تهديدات أو احتيال.",
                        DisplayOrder = 3
                    }
                }
            };

            await context.PrivacyPolicies.AddAsync(privacyPolicy);
            await context.SaveChangesAsync();
        }
    }
}

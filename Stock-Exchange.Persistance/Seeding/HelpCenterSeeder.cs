using Bogus;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Persistance.Seeding
{
    public static class HelpCenterSeeder
    {
        public static async Task SeedHelpCenterAsync(StockExchangeDbContext context)
        {
            if (await context.HelpCenters.AnyAsync())
            {
                return;
            }

            var categories = await context.HelpCenterCategories.ToListAsync();
            if (!categories.Any())
            {
                categories = await HelpCenterCategorySeeder.SeedCategoriesAsync(context);
            }

            var categoryDict = categories.ToDictionary(c => c.TitleEn, c => c.Id);

            var items = new List<HelpCenter>();
            int order = 1;

            void AddArticle(string categoryTitle, string titleEn, string titleAr, string contentEn, string contentAr)
            {
                Guid? catId = categoryDict.TryGetValue(categoryTitle, out var id) ? id : null;
                items.Add(new HelpCenter
                {
                    CategoryId = catId,
                    TitleEn = titleEn,
                    TitleAr = titleAr,
                    ContentEn = contentEn,
                    ContentAr = contentAr,
                    DisplayOrder = order++
                });
            }

            // Account & Verification
            AddArticle(
                "Account & Verification",
                "How do I complete identity verification (KYC)?",
                "كيف يمكنني إتمام عملية التحقق من الهوية (KYC)؟",
                "To verify your identity, navigate to your Profile Settings and select 'Verification'. Submit a government-issued ID (Passport, National ID, or Driver's License) along with a clear selfie. Make sure the document details match your account information.",
                "للتحقق من هويتك، انتقل إلى إعدادات الملف الشخصي واختر 'التحقق من الهوية'. قم برفع وثيقة هوية صادرة عن جهة حكومية (جواز سفر أو بطاقة شخصية أو رخصة قيادة) مع صورة شخصية حديثة وواضحة. تأكد من تطابق بيانات الوثيقة مع بيانات حسابك."
            );

            AddArticle(
                "Account & Verification",
                "How long does account verification take?",
                "كم من الوقت يستغرق التحقق من الحساب؟",
                "Most verification requests are processed automatically within 15 to 30 minutes. In cases requiring manual review by our compliance team, it may take up to 24-48 business hours. You will receive an email notification once approved.",
                "تتم معالجة معظم طلبات التحقق تلقائياً في غضون 15 إلى 30 دقيقة. في بعض الحالات التي تتطلب مراجعة يدوية من فريق الامتثال، قد تستغرق العملية ما بين 24 إلى 48 ساعة عمل. ستتلقى إشعاراً فور اكتمال المراجعة."
            );

            AddArticle(
                "Account & Verification",
                "Can I update my registered email or phone number?",
                "هل يمكنني تحديث البريد الإلكتروني أو رقم الهاتف المسجل؟",
                "Yes, you can update your contact details under Security Settings. For your protection, changing sensitive credentials triggers a temporary 24-hour withdrawal lock.",
                "نعم، يمكنك تحديث بيانات الاتصال الخاصة بك من خلال إعدادات الأمان. لحماية حسابك، يتم تطبيق قفل مؤقت على عمليات السحب لمدة 24 ساعة بعد تعديل البيانات الحساسة."
            );

            // Trading & Orders
            AddArticle(
                "Trading & Orders",
                "What is the difference between Market and Limit orders?",
                "ما هو الفرق بين أمر السوق (Market) وأمر الحد (Limit)؟",
                "A Market Order executes immediately at the best available current market price. A Limit Order allows you to specify the maximum price you want to buy or the minimum price you want to sell, executing only when the market reaches your target.",
                "أمر السوق (Market Order) يُنفذ فورياً بأفضل سعر متاح حالياً في السوق. أما أمر الحد (Limit Order) فيتيح لك تحديد السعر الأقصى للشراء أو الأدنى للبيع، ولا يُنفذ إلا إذا وصل سعر السوق إلى السعر الذي حددته."
            );

            AddArticle(
                "Trading & Orders",
                "How do I cancel an open order?",
                "كيف يمكنني إلغاء أمر تداول مفتوح؟",
                "Go to the Trading Terminal, scroll to the 'Open Orders' tab at the bottom, find the order you wish to cancel, and click 'Cancel'. Unfilled funds will instantly return to your available balance.",
                "انتقل إلى منصة التداول، ثم توجه إلى تبويب 'الأوامر المفتوحة' في الأسفل، وحدد الأمر الذي تريد إلغاءه واضغط على 'إلغاء'. ستعود الأموال غير المنفذة فوراً إلى رصيدك المتاح."
            );

            AddArticle(
                "Trading & Orders",
                "What is price slippage and how can I avoid it?",
                "ما هو الانزلاق السعري وكيف يمكنني تفاديه؟",
                "Slippage occurs when market prices shift between the time an order is submitted and when it executes, common during high volatility. To minimize slippage, use Limit Orders instead of Market Orders.",
                "يحدث الانزلاق السعري عندما يتغير السعر في السوق بين وقت إرسال الأمر وتوقيت تنفيذه، خاصة أثناء فترات التقلبات العالية. للتقليل من ذلك، يُنصح باستخدام أوامر الحد (Limit) بدلاً من أوامر السوق."
            );

            // Deposits & Withdrawals
            AddArticle(
                "Deposits & Withdrawals",
                "How do I deposit funds via bank transfer?",
                "كيف يمكنني إيداع الأموال عبر التحويل البنكي؟",
                "Go to 'Wallet' > 'Deposit', choose Bank Transfer, select your currency, and copy the provided IBAN and reference code. Always include your reference code in the transfer description to ensure prompt crediting.",
                "انتقل إلى 'المحفظة' > 'إيداع'، واختر التحويل البنكي والعملة المراد إيداعها، ثم انسخ رقم الآيبان (IBAN) وكود المرجع المخصص لك. احرص دائماً على تضمين كود المرجع في وصف التحويل لضمان سرعة إضافة الرصيد."
            );

            AddArticle(
                "Deposits & Withdrawals",
                "What are the withdrawal processing times and daily limits?",
                "ما هي فترات معالجة عمليات السحب والحدود اليومية؟",
                "Standard withdrawals are reviewed and dispatched within 1 to 4 hours. Daily limits depend on your KYC verification tier, ranging from $10,000/day for Tier 1 up to $500,000/day for fully verified accounts.",
                "تتم مراجعة ومعالجة عمليات السحب القياسية في غضون 1 إلى 4 ساعات. تعتمد الحدود اليومية على مستوى التحقق من الحساب (KYC)، وتبدأ من 10,000 دولار يومياً للمستوى الأول وتصل إلى 500,000 دولار يومياً للحسابات المكتملة."
            );

            AddArticle(
                "Deposits & Withdrawals",
                "Why is my withdrawal status marked as pending?",
                "لماذا تظهر حالة عملية السحب كمعلقة؟",
                "Pending status indicates the withdrawal is undergoing standard automated security risk checks. If additional documentation is required, our support team will contact you via email.",
                "تعني الحالة 'معلقة' أن العملية تخضع لفحوصات الأمان الآلية المعتادة. إذا تطلب الأمر أي تأكيدات إضافية، سيتواصل معك فريق الدعم عبر بريدك الإلكتروني المسجل."
            );

            // Security & Two-Factor Authentication
            AddArticle(
                "Security & Two-Factor Authentication",
                "How do I enable Two-Factor Authentication (2FA)?",
                "كيف أقوم بتفعيل المصادقة الثنائية (2FA)؟",
                "Go to Security Settings > Two-Factor Authentication, install Google Authenticator or Microsoft Authenticator, scan the displayed QR code, and enter the 6-digit verification code to confirm activation.",
                "توجه إلى إعدادات الأمان > المصادقة الثنائية، ثم قم بتثبيت تطبيق Google Authenticator أو Microsoft Authenticator، وامسح رمز الاستجابة السريعة (QR)، وأدخل الرمز المكون من 6 أرقام لتأكيد التفعيل."
            );

            AddArticle(
                "Security & Two-Factor Authentication",
                "What should I do if I lose access to my 2FA device?",
                "ماذا أفعل إذا فقدت جهازي المستخدم في المصادقة الثنائية؟",
                "If you saved your 16-digit backup key during setup, you can restore it directly on any authenticator app. If not, click 'Reset 2FA' on the login screen to start the identity verification recovery flow.",
                "إذا كنت قد حفظت مفتاح النسخ الاحتياطي المكون من 16 حرفاً ورقم أثناء الإعداد، يمكنك استعادته مباشرة في أي جهاز. بخلاف ذلك، اضغط على 'إعادة تعيين 2FA' في شاشة تسجيل الدخول لبدء إجراءات استعادة الحساب."
            );

            // Fees, Limits & VIP Levels
            AddArticle(
                "Fees, Limits & VIP Levels",
                "Where can I find the trading fee schedule?",
                "أين يمكنني الاطلاع على جدول رسوم التداول؟",
                "Trading fees start at 0.10% for makers and 0.15% for takers. You can view our comprehensive tier structure and discount rates on the Fees page accessible from the website footer.",
                "تبدأ رسوم التداول من 0.10% لصناع السوق (Maker) و0.15% للمستجيبين (Taker). يمكنك الاطلاع على الجدول الكامل للرسوم والتخفيضات المتاحة عبر صفحة الرسوم الموجودة في أسفل الموقع."
            );

            AddArticle(
                "Fees, Limits & VIP Levels",
                "How do I achieve a higher VIP tier?",
                "كيف يمكنني الترقية إلى مستوى VIP أعلى؟",
                "VIP tiers are calculated automatically at midnight UTC based on your trailing 30-day trading volume or your average daily platform balance. Higher tiers enjoy lower trading fees and higher withdrawal limits.",
                "تُحسب مستويات VIP تلقائياً عند منتصف الليل بتوقيت UTC بناءً على حجم تداولك خلال آخر 30 يوماً أو متوسط رصيدك في المنصة. تمنحك المستويات الأعلى تخفيضات في الرسوم وزيادة في حدود السحب."
            );

            // Mobile & Web Applications
            AddArticle(
                "Mobile & Web Applications",
                "Where can I download the official mobile apps?",
                "من أين يمكنني تحميل تطبيقات الهاتف الرسمية؟",
                "Our mobile app is available for iOS on the Apple App Store and for Android on Google Play. Always verify that the publisher is 'Stock Exchange Official' before downloading.",
                "تطبيقنا متاح لنظام iOS على متجر Apple App Store ولنظام Android على متجر Google Play. احرص دائماً على التأكد من أن الناشر هو 'Stock Exchange Official' قبل التحميل."
            );

            AddArticle(
                "Mobile & Web Applications",
                "Troubleshooting real-time chart connection issues",
                "حل مشكلات اتصال وتحديث الرسوم البيانية المباشرة",
                "If market depth or candlestick charts are not streaming, try hard refreshing your browser (Ctrl+F5 or Cmd+Shift+R), disable conflicting browser extensions, or ensure your firewall permits WebSocket connections.",
                "إذا كانت الرسوم البيانية أو عمق السوق لا يُحدث في الوقت الفعلي، جرّب تحديث الصفحة بالكامل (Ctrl+F5)، أو تعطيل إضافات المتصفح التي قد تحجب البيانات، وتأكد من سماح الجدار الناري باتصالات WebSocket."
            );

            await context.HelpCenters.AddRangeAsync(items);
            await context.SaveChangesAsync();
        }
    }
}

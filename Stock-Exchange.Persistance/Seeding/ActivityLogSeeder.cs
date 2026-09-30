using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Persistance.Seeding
{
    public static class ActivityLogSeeder
    {
        public static async Task SeedActivityLogsAsync(StockExchangeDbContext context)
        {
            if (await context.ActivityLogs.IgnoreQueryFilters().AnyAsync())
                return;

            var logs = new List<ActivityLog>
            {
                new ActivityLog
                {
                    FormattedId = "act-001",
                    UserName = "أحمد محمد",
                    UserEmail = "ahmed.m@gmail.com",
                    Action = "اشترك بالخطة الاحترافية السنوية (120.00$)",
                    ActionAr = "اشترك بالخطة الاحترافية السنوية (120.00$)",
                    ActionEn = "Subscribed to Annual Professional Plan ($120.00)",
                    ResourceType = ActivityResourceType.SubscriptionPlans,
                    IpAddress = "197.38.12.94",
                    Device = "Chrome on macOS"
                },
                new ActivityLog
                {
                    FormattedId = "act-002",
                    UserName = "سارة علي",
                    UserEmail = "sara.ali@outlook.com",
                    Action = "أنشأت حساب جديد عبر البريد الإلكتروني",
                    ActionAr = "أنشأت حساب جديد عبر البريد الإلكتروني",
                    ActionEn = "Created new account via email",
                    ResourceType = ActivityResourceType.UserRegistrations,
                    IpAddress = "156.204.88.19",
                    Device = "Safari on iPhone 15"
                },
                new ActivityLog
                {
                    FormattedId = "act-003",
                    UserName = "المدير (أنت)",
                    UserEmail = "admin@finwise.com",
                    Action = "نشر مقال تعليمي 'الذهب يسجل أعلى مستوياته في التاريخ'",
                    ActionAr = "نشر مقال تعليمي 'الذهب يسجل أعلى مستوياته في التاريخ'",
                    ActionEn = "Published educational article 'Gold records all-time highs'",
                    ResourceType = ActivityResourceType.Articles,
                    IpAddress = "192.168.1.1",
                    Device = "Firefox on Windows 11"
                },
                new ActivityLog
                {
                    FormattedId = "act-004",
                    UserName = "محمد حسن",
                    UserEmail = "m.hassan@finwise.com",
                    Action = "رفع فيديو تحليل فني جديد 'اختراق مؤشر تاسي لمستوى المقاومة'",
                    ActionAr = "رفع فيديو تحليل فني جديد 'اختراق مؤشر تاسي لمستوى المقاومة'",
                    ActionEn = "Uploaded technical analysis video 'TASI Index breakout'",
                    ResourceType = ActivityResourceType.Videos,
                    IpAddress = "41.233.10.4",
                    Device = "Chrome on Windows"
                },
                new ActivityLog
                {
                    FormattedId = "act-005",
                    UserName = "خالد المنصور",
                    UserEmail = "khalid.m@gmail.com",
                    Action = "أنشأ حساب جديد عبر الموقع الإلكتروني",
                    ActionAr = "أنشأ حساب جديد عبر الموقع الإلكتروني",
                    ActionEn = "Created new account via website",
                    ResourceType = ActivityResourceType.UserRegistrations,
                    IpAddress = "185.140.24.11",
                    Device = "Safari on macOS"
                },
                new ActivityLog
                {
                    FormattedId = "act-006",
                    UserName = "ليلى أحمد",
                    UserEmail = "layla.a@gmail.com",
                    Action = "نشر مقال تعليمي 'أساسيات التحليل الفني للمبتدئين'",
                    ActionAr = "نشر مقال تعليمي 'أساسيات التحليل الفني للمبتدئين'",
                    ActionEn = "Published educational article 'Technical Analysis Basics for Beginners'",
                    ResourceType = ActivityResourceType.Articles,
                    IpAddress = "197.38.12.94",
                    Device = "Chrome on Windows"
                },
                new ActivityLog
                {
                    FormattedId = "act-007",
                    UserName = "طارق العلي",
                    UserEmail = "tareq.ali@outlook.com",
                    Action = "رفع فيديو شرح استراتيجية التداول اليومي",
                    ActionAr = "رفع فيديو شرح استراتيجية التداول اليومي",
                    ActionEn = "Uploaded day trading strategy guide video",
                    ResourceType = ActivityResourceType.Videos,
                    IpAddress = "82.165.197.1",
                    Device = "Edge on Windows 11"
                },
                new ActivityLog
                {
                    FormattedId = "act-008",
                    UserName = "المدير (أنت)",
                    UserEmail = "admin@finwise.com",
                    Action = "تحديث خطة الاشتراك الشهرية",
                    ActionAr = "تحديث خطة الاشتراك الشهرية",
                    ActionEn = "Updated monthly subscription plan details",
                    ResourceType = ActivityResourceType.SubscriptionPlans,
                    IpAddress = "192.168.1.1",
                    Device = "Firefox on Windows 11"
                }
            };

            for (int i = 0; i < logs.Count; i++)
            {
                logs[i].MarkAsCreated("System", DateTime.UtcNow.AddMinutes(-(i + 1) * 35));
            }

            await context.ActivityLogs.AddRangeAsync(logs);
            await context.SaveChangesAsync();
        }
    }
}

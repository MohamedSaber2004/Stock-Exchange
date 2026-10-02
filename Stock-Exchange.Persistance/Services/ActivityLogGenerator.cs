using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Persistance.Services
{
    public class ActivityLogGenerator : IActivityLogGenerator
    {
        private readonly ICurrentUserService _currentUserService;

        public ActivityLogGenerator(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public List<ActivityLog> GenerateActivityLogs(ChangeTracker changeTracker)
        {
            var logs = new List<ActivityLog>();

            var isAuth = _currentUserService?.IsAuthenticated ?? false;
            var currentUserId = isAuth && _currentUserService?.UserId != Guid.Empty
                ? _currentUserService?.UserId
                : (Guid?)null;

            var currentUserName = _currentUserService?.FullName;
            if (string.IsNullOrWhiteSpace(currentUserName))
                currentUserName = _currentUserService?.Email;
            if (string.IsNullOrWhiteSpace(currentUserName))
                currentUserName = isAuth ? "System Administrator" : "System";

            var currentUserEmail = _currentUserService?.Email ?? string.Empty;
            var ipAddress = _currentUserService?.IpAddress ?? "127.0.0.1";
            var device = _currentUserService?.Device ?? "Web";

            var entries = changeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
                .ToList();

            foreach (var entry in entries)
            {
                if (entry.Entity is ActivityLog) continue;
                if (entry.Entity is UserRefreshToken) continue;
                if (entry.Entity.GetType().Namespace?.StartsWith("Microsoft.AspNetCore.Identity") == true && entry.Entity is not ApplicationUser) continue;

                var op = DetermineOperation(entry);
                if (op == ChangeOperation.None) continue;

                var meta = MapEntityToLogMeta(entry, op, isAuth, currentUserId, currentUserName);
                if (!meta.ResourceType.HasValue) continue;

                var formattedId = $"act-{DateTime.UtcNow:yyMMddHHmmss}-{Random.Shared.Next(100, 999)}";

                var log = new ActivityLog
                {
                    FormattedId = formattedId,
                    UserId = meta.LogUserId ?? currentUserId,
                    UserName = !string.IsNullOrWhiteSpace(meta.LogUserName) ? meta.LogUserName : currentUserName,
                    UserEmail = !string.IsNullOrWhiteSpace(meta.LogUserEmail) ? meta.LogUserEmail : currentUserEmail,
                    UserProfilePictureUrl = meta.UserProfilePictureUrl,
                    Action = meta.ActionEn,
                    ActionAr = meta.ActionAr,
                    ActionEn = meta.ActionEn,
                    ResourceType = meta.ResourceType.Value,
                    IpAddress = ipAddress,
                    Device = device,
                    Details = meta.DetailsEn,
                    DetailsAr = meta.DetailsAr,
                    DetailsEn = meta.DetailsEn
                };

                logs.Add(log);
            }

            return logs;
        }

        private enum ChangeOperation
        {
            None,
            Added,
            Modified,
            Deleted
        }

        private static ChangeOperation DetermineOperation(EntityEntry entry)
        {
            if (entry.State == EntityState.Added)
                return ChangeOperation.Added;

            if (entry.State == EntityState.Deleted)
                return ChangeOperation.Deleted;

            if (entry.State == EntityState.Modified)
            {
                var isDeletedProp = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "IsDeleted");
                if (isDeletedProp != null && isDeletedProp.IsModified && isDeletedProp.CurrentValue is true && isDeletedProp.OriginalValue is false)
                {
                    return ChangeOperation.Deleted;
                }

                var modifiedProps = entry.Properties.Where(p => p.IsModified &&
                    p.Metadata.Name != "UpdatedAt" &&
                    p.Metadata.Name != "UpdatedBy" &&
                    p.Metadata.Name != "DeletedAt" &&
                    p.Metadata.Name != "DeletedBy" &&
                    p.Metadata.Name != "Version").ToList();

                if (modifiedProps.Count == 0)
                    return ChangeOperation.None;

                return ChangeOperation.Modified;
            }

            return ChangeOperation.None;
        }

        private static EntityLogMeta MapEntityToLogMeta(
            EntityEntry entry,
            ChangeOperation op,
            bool isAuth,
            Guid? currentUserId,
            string currentUserName)
        {
            return entry.Entity switch
            {
                ApplicationUser user => MapUserLog(entry, user, op, isAuth, currentUserId, currentUserName),
                Article article => MapArticleLog(article, op),
                Video video => MapVideoLog(video, op),
                News news => MapNewsLog(news, op),
                SubscriptionPlan plan => MapSubscriptionPlanLog(plan, op),
                PlanFeature planFeature => MapPlanFeatureLog(planFeature, op),
                HelpCenter helpCenter => MapHelpCenterLog(helpCenter, op),
                HelpCenterCategory category => MapHelpCenterCategoryLog(category, op),
                Domain.Entities.AboutUs => new EntityLogMeta(
                    ActivityResourceType.AboutUs,
                    op == ChangeOperation.Added ? "إضافة محتوى من نحن" : (op == ChangeOperation.Deleted ? "حذف محتوى من نحن" : "تعديل محتوى من نحن"),
                    op == ChangeOperation.Added ? "Added About Us content" : (op == ChangeOperation.Deleted ? "Deleted About Us content" : "Updated About Us content"),
                    "تم تحديث بيانات صفحة من نحن",
                    "About Us page information updated"),
                AboutUsFeature feature => MapAboutUsFeatureLog(feature, op),
                Domain.Entities.TermsAndConditions => new EntityLogMeta(
                    ActivityResourceType.TermsAndConditions,
                    op == ChangeOperation.Added ? "إضافة الشروط والأحكام" : (op == ChangeOperation.Deleted ? "حذف الشروط والأحكام" : "تعديل الشروط والأحكام"),
                    op == ChangeOperation.Added ? "Added Terms & Conditions" : (op == ChangeOperation.Deleted ? "Deleted Terms & Conditions" : "Updated Terms & Conditions"),
                    "تم تحديث بيانات الشروط والأحكام",
                    "Terms & Conditions updated"),
                TermsAndConditionsSection section => MapTermsSectionLog(section, op),
                Domain.Entities.PrivacyPolicy => new EntityLogMeta(
                    ActivityResourceType.PrivacyPolicy,
                    op == ChangeOperation.Added ? "إضافة سياسة الخصوصية" : (op == ChangeOperation.Deleted ? "حذف سياسة الخصوصية" : "تعديل سياسة الخصوصية"),
                    op == ChangeOperation.Added ? "Added Privacy Policy" : (op == ChangeOperation.Deleted ? "Deleted Privacy Policy" : "Updated Privacy Policy"),
                    "تم تحديث بيانات سياسة الخصوصية",
                    "Privacy Policy updated"),
                PrivacyPolicySection privacySection => MapPrivacySectionLog(privacySection, op),
                _ => new EntityLogMeta(null, string.Empty, string.Empty, string.Empty, string.Empty)
            };
        }

        private static EntityLogMeta MapUserLog(
            EntityEntry entry,
            ApplicationUser user,
            ChangeOperation op,
            bool isAuth,
            Guid? currentUserId,
            string currentUserName)
        {
            var userDisplayName = !string.IsNullOrWhiteSpace(user.FullName) ? user.FullName : (user.UserName ?? user.Email ?? "User");

            if (op == ChangeOperation.Added)
            {
                var isSelfRegistration = !isAuth || (currentUserId.HasValue && currentUserId.Value == user.Id);
                if (isSelfRegistration)
                {
                    return new EntityLogMeta(
                        ActivityResourceType.UserRegistrations,
                        $"تسجيل مستخدم جديد: {userDisplayName}",
                        $"New user registered: {userDisplayName}",
                        $"تم تسجيل حساب مستخدم جديد بالبريد: {user.Email}",
                        $"New user registered with email: {user.Email}",
                        LogUserId: user.Id,
                        LogUserName: userDisplayName,
                        LogUserEmail: user.Email,
                        UserProfilePictureUrl: user.ProfilePictureUrl);
                }

                return new EntityLogMeta(
                    ActivityResourceType.Users,
                    $"إضافة مستخدم جديد: {userDisplayName}",
                    $"Added new user: {userDisplayName}",
                    $"تم إنشاء حساب مستخدم بواسطة الإدارة ({user.Email})",
                    $"User account created by admin ({user.Email})",
                    LogUserId: user.Id,
                    LogUserName: userDisplayName,
                    LogUserEmail: user.Email,
                    UserProfilePictureUrl: user.ProfilePictureUrl);
            }

            if (op == ChangeOperation.Deleted)
            {
                return new EntityLogMeta(
                    ActivityResourceType.Users,
                    $"حذف حساب المستخدم: {userDisplayName}",
                    $"Deleted user account: {userDisplayName}",
                    $"تم حذف حساب المستخدم {userDisplayName} ({user.Email})",
                    $"Deleted user account for {userDisplayName} ({user.Email})",
                    LogUserId: user.Id,
                    LogUserName: userDisplayName,
                    LogUserEmail: user.Email,
                    UserProfilePictureUrl: user.ProfilePictureUrl);
            }

            // Modified
            var passwordProp = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "PasswordHash");
            if (passwordProp != null && passwordProp.IsModified)
            {
                var origHash = passwordProp.OriginalValue as string;
                var currHash = passwordProp.CurrentValue as string;
                if (!string.IsNullOrEmpty(origHash) && !string.IsNullOrEmpty(currHash) && origHash != currHash && isAuth)
                {
                    return new EntityLogMeta(
                        ActivityResourceType.Users,
                        $"تغيير كلمة المرور للمستخدم: {userDisplayName}",
                        $"Changed password for user: {userDisplayName}",
                        $"تم تغيير كلمة المرور للمستخدم {userDisplayName} ({user.Email})",
                        $"Changed password for user {userDisplayName} ({user.Email})",
                        LogUserId: user.Id,
                        LogUserName: userDisplayName,
                        LogUserEmail: user.Email,
                        UserProfilePictureUrl: user.ProfilePictureUrl);
                }
            }

            var activeProp = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "IsActive");
            if (activeProp != null && activeProp.IsModified)
            {
                var stateAr = user.IsActive ? "تنشيط" : "تعطيل";
                var stateEn = user.IsActive ? "Activated" : "Deactivated";
                return new EntityLogMeta(
                    ActivityResourceType.Users,
                    $"{stateAr} حساب المستخدم: {userDisplayName}",
                    $"{stateEn} user account: {userDisplayName}",
                    $"تم {stateAr} حساب المستخدم {userDisplayName} ({user.Email})",
                    $"{stateEn} account for user {userDisplayName} ({user.Email})",
                    LogUserId: user.Id,
                    LogUserName: userDisplayName,
                    LogUserEmail: user.Email,
                    UserProfilePictureUrl: user.ProfilePictureUrl);
            }

            return new EntityLogMeta(
                ActivityResourceType.Users,
                $"تعديل بيانات المستخدم: {userDisplayName}",
                $"Updated user details: {userDisplayName}",
                $"تم تعديل بيانات المستخدم {userDisplayName} ({user.Email})",
                $"Updated details for user {userDisplayName} ({user.Email})",
                LogUserId: user.Id,
                LogUserName: userDisplayName,
                LogUserEmail: user.Email,
                UserProfilePictureUrl: user.ProfilePictureUrl);
        }

        private static EntityLogMeta MapArticleLog(Article a, ChangeOperation op)
        {
            var titleAr = !string.IsNullOrWhiteSpace(a.TitleAr) ? a.TitleAr : a.TitleEn;
            var titleEn = !string.IsNullOrWhiteSpace(a.TitleEn) ? a.TitleEn : a.TitleAr;
            return op switch
            {
                ChangeOperation.Added => new EntityLogMeta(
                    ActivityResourceType.Articles,
                    $"إضافة مقال جديد: {titleAr}",
                    $"Added new article: {titleEn}",
                    $"تم إضافة المقال بواسطة الكاتب: {a.AuthorName}",
                    $"Article added by author: {a.AuthorName}"),
                ChangeOperation.Deleted => new EntityLogMeta(
                    ActivityResourceType.Articles,
                    $"حذف مقال: {titleAr}",
                    $"Deleted article: {titleEn}",
                    $"تم حذف المقال: {titleAr}",
                    $"Deleted article: {titleEn}"),
                _ => new EntityLogMeta(
                    ActivityResourceType.Articles,
                    $"تعديل مقال: {titleAr}",
                    $"Updated article: {titleEn}",
                    $"تم تعديل بيانات المقال: {titleAr}",
                    $"Updated article details: {titleEn}")
            };
        }

        private static EntityLogMeta MapVideoLog(Video v, ChangeOperation op)
        {
            var titleAr = !string.IsNullOrWhiteSpace(v.TitleAr) ? v.TitleAr : v.TitleEn;
            var titleEn = !string.IsNullOrWhiteSpace(v.TitleEn) ? v.TitleEn : v.TitleAr;
            return op switch
            {
                ChangeOperation.Added => new EntityLogMeta(
                    ActivityResourceType.Videos,
                    $"إضافة فيديو جديد: {titleAr}",
                    $"Added new video: {titleEn}",
                    $"تمت إضافة الفيديو بنجاح",
                    $"Video added successfully"),
                ChangeOperation.Deleted => new EntityLogMeta(
                    ActivityResourceType.Videos,
                    $"حذف فيديو: {titleAr}",
                    $"Deleted video: {titleEn}",
                    $"تم حذف الفيديو: {titleAr}",
                    $"Deleted video: {titleEn}"),
                _ => new EntityLogMeta(
                    ActivityResourceType.Videos,
                    $"تعديل فيديو: {titleAr}",
                    $"Updated video: {titleEn}",
                    $"تم تحديث بيانات الفيديو: {titleAr}",
                    $"Updated video details: {titleEn}")
            };
        }

        private static EntityLogMeta MapNewsLog(News n, ChangeOperation op)
        {
            var titleAr = !string.IsNullOrWhiteSpace(n.TitleAr) ? n.TitleAr : n.TitleEn;
            var titleEn = !string.IsNullOrWhiteSpace(n.TitleEn) ? n.TitleEn : n.TitleAr;
            return op switch
            {
                ChangeOperation.Added => new EntityLogMeta(
                    ActivityResourceType.News,
                    $"إضافة خبر جديد: {titleAr}",
                    $"Added news: {titleEn}",
                    $"تمت إضافة الخبر بنجاح",
                    $"News article added successfully"),
                ChangeOperation.Deleted => new EntityLogMeta(
                    ActivityResourceType.News,
                    $"حذف خبر: {titleAr}",
                    $"Deleted news: {titleEn}",
                    $"تم حذف الخبر: {titleAr}",
                    $"Deleted news article: {titleEn}"),
                _ => new EntityLogMeta(
                    ActivityResourceType.News,
                    $"تعديل خبر: {titleAr}",
                    $"Updated news: {titleEn}",
                    $"تم تحديث بيانات الخبر: {titleAr}",
                    $"Updated news details: {titleEn}")
            };
        }

        private static EntityLogMeta MapSubscriptionPlanLog(SubscriptionPlan plan, ChangeOperation op)
        {
            var titleAr = !string.IsNullOrWhiteSpace(plan.NameAr) ? plan.NameAr : plan.NameEn;
            var titleEn = !string.IsNullOrWhiteSpace(plan.NameEn) ? plan.NameEn : plan.NameAr;
            return op switch
            {
                ChangeOperation.Added => new EntityLogMeta(
                    ActivityResourceType.SubscriptionPlans,
                    $"إضافة خطة اشتراك جديدة: {titleAr}",
                    $"Added subscription plan: {titleEn}",
                    $"تمت إضافة خطة الاشتراك بسعر {plan.PriceEgp}",
                    $"Subscription plan added with price {plan.PriceEgp}"),
                ChangeOperation.Deleted => new EntityLogMeta(
                    ActivityResourceType.SubscriptionPlans,
                    $"حذف خطة اشتراك: {titleAr}",
                    $"Deleted subscription plan: {titleEn}",
                    $"تم حذف خطة الاشتراك: {titleAr}",
                    $"Deleted subscription plan: {titleEn}"),
                _ => new EntityLogMeta(
                    ActivityResourceType.SubscriptionPlans,
                    $"تعديل خطة اشتراك: {titleAr}",
                    $"Updated subscription plan: {titleEn}",
                    $"تم تحديث خطة الاشتراك: {titleAr}",
                    $"Updated subscription plan: {titleEn}")
            };
        }

        private static EntityLogMeta MapPlanFeatureLog(PlanFeature pf, ChangeOperation op)
        {
            var titleAr = !string.IsNullOrWhiteSpace(pf.TextAr) ? pf.TextAr : pf.TextEn;
            var titleEn = !string.IsNullOrWhiteSpace(pf.TextEn) ? pf.TextEn : pf.TextAr;
            return op switch
            {
                ChangeOperation.Added => new EntityLogMeta(
                    ActivityResourceType.SubscriptionPlans,
                    $"إضافة ميزة لخطة الاشتراك: {titleAr}",
                    $"Added plan feature: {titleEn}",
                    $"تمت إضافة ميزة جديدة لخطة الاشتراك",
                    $"Added feature to subscription plan"),
                ChangeOperation.Deleted => new EntityLogMeta(
                    ActivityResourceType.SubscriptionPlans,
                    $"حذف ميزة من خطة الاشتراك: {titleAr}",
                    $"Deleted plan feature: {titleEn}",
                    $"تم حذف الميزة من خطة الاشتراك",
                    $"Deleted feature from subscription plan"),
                _ => new EntityLogMeta(
                    ActivityResourceType.SubscriptionPlans,
                    $"تعديل ميزة في خطة الاشتراك: {titleAr}",
                    $"Updated plan feature: {titleEn}",
                    $"تم تحديث ميزة خطة الاشتراك",
                    $"Updated feature in subscription plan")
            };
        }

        private static EntityLogMeta MapHelpCenterLog(HelpCenter hc, ChangeOperation op)
        {
            var titleAr = !string.IsNullOrWhiteSpace(hc.TitleAr) ? hc.TitleAr : hc.TitleEn;
            var titleEn = !string.IsNullOrWhiteSpace(hc.TitleEn) ? hc.TitleEn : hc.TitleAr;
            return op switch
            {
                ChangeOperation.Added => new EntityLogMeta(
                    ActivityResourceType.HelpCenter,
                    $"إضافة سؤال في مركز المساعدة: {titleAr}",
                    $"Added help center item: {titleEn}",
                    $"تمت إضافة العنصر بنجاح إلى مركز المساعدة",
                    $"Item added successfully to help center"),
                ChangeOperation.Deleted => new EntityLogMeta(
                    ActivityResourceType.HelpCenter,
                    $"حذف سؤال من مركز المساعدة: {titleAr}",
                    $"Deleted help center item: {titleEn}",
                    $"تم حذف العنصر من مركز المساعدة",
                    $"Deleted item from help center"),
                _ => new EntityLogMeta(
                    ActivityResourceType.HelpCenter,
                    $"تعديل سؤال في مركز المساعدة: {titleAr}",
                    $"Updated help center item: {titleEn}",
                    $"تم تحديث بيانات العنصر في مركز المساعدة",
                    $"Updated item in help center")
            };
        }

        private static EntityLogMeta MapHelpCenterCategoryLog(HelpCenterCategory category, ChangeOperation op)
        {
            var titleAr = !string.IsNullOrWhiteSpace(category.TitleAr) ? category.TitleAr : category.TitleEn;
            var titleEn = !string.IsNullOrWhiteSpace(category.TitleEn) ? category.TitleEn : category.TitleAr;
            return op switch
            {
                ChangeOperation.Added => new EntityLogMeta(
                    ActivityResourceType.HelpCenter,
                    $"إضافة تصنيف في مركز المساعدة: {titleAr}",
                    $"Added help center category: {titleEn}",
                    $"تمت إضافة تصنيف جديد في مركز المساعدة",
                    $"Added new category to help center"),
                ChangeOperation.Deleted => new EntityLogMeta(
                    ActivityResourceType.HelpCenter,
                    $"حذف تصنيف من مركز المساعدة: {titleAr}",
                    $"Deleted help center category: {titleEn}",
                    $"تم حذف التصنيف من مركز المساعدة",
                    $"Deleted category from help center"),
                _ => new EntityLogMeta(
                    ActivityResourceType.HelpCenter,
                    $"تعديل تصنيف في مركز المساعدة: {titleAr}",
                    $"Updated help center category: {titleEn}",
                    $"تم تحديث بيانات التصنيف في مركز المساعدة",
                    $"Updated category in help center")
            };
        }

        private static EntityLogMeta MapAboutUsFeatureLog(AboutUsFeature feature, ChangeOperation op)
        {
            var titleAr = !string.IsNullOrWhiteSpace(feature.TitleAr) ? feature.TitleAr : feature.TitleEn;
            var titleEn = !string.IsNullOrWhiteSpace(feature.TitleEn) ? feature.TitleEn : feature.TitleAr;
            return op switch
            {
                ChangeOperation.Added => new EntityLogMeta(
                    ActivityResourceType.AboutUs,
                    $"إضافة ميزة في من نحن: {titleAr}",
                    $"Added About Us feature: {titleEn}",
                    $"تمت إضافة ميزة جديدة في صفحة من نحن",
                    $"Added new feature in About Us"),
                ChangeOperation.Deleted => new EntityLogMeta(
                    ActivityResourceType.AboutUs,
                    $"حذف ميزة من من نحن: {titleAr}",
                    $"Deleted About Us feature: {titleEn}",
                    $"تم حذف الميزة من صفحة من نحن",
                    $"Deleted feature from About Us"),
                _ => new EntityLogMeta(
                    ActivityResourceType.AboutUs,
                    $"تعديل ميزة في من نحن: {titleAr}",
                    $"Updated About Us feature: {titleEn}",
                    $"تم تحديث الميزة في صفحة من نحن",
                    $"Updated feature in About Us")
            };
        }

        private static EntityLogMeta MapTermsSectionLog(TermsAndConditionsSection section, ChangeOperation op)
        {
            var titleAr = !string.IsNullOrWhiteSpace(section.TitleAr) ? section.TitleAr : section.TitleEn;
            var titleEn = !string.IsNullOrWhiteSpace(section.TitleEn) ? section.TitleEn : section.TitleAr;
            return op switch
            {
                ChangeOperation.Added => new EntityLogMeta(
                    ActivityResourceType.TermsAndConditions,
                    $"إضافة بند في الشروط والأحكام: {titleAr}",
                    $"Added Terms & Conditions section: {titleEn}",
                    $"تمت إضافة بند جديد في الشروط والأحكام",
                    $"Added section in Terms & Conditions"),
                ChangeOperation.Deleted => new EntityLogMeta(
                    ActivityResourceType.TermsAndConditions,
                    $"حذف بند من الشروط والأحكام: {titleAr}",
                    $"Deleted Terms & Conditions section: {titleEn}",
                    $"تم حذف البند من الشروط والأحكام",
                    $"Deleted section from Terms & Conditions"),
                _ => new EntityLogMeta(
                    ActivityResourceType.TermsAndConditions,
                    $"تعديل بند في الشروط والأحكام: {titleAr}",
                    $"Updated Terms & Conditions section: {titleEn}",
                    $"تم تحديث البند في الشروط والأحكام",
                    $"Updated section in Terms & Conditions")
            };
        }

        private static EntityLogMeta MapPrivacySectionLog(PrivacyPolicySection section, ChangeOperation op)
        {
            var titleAr = !string.IsNullOrWhiteSpace(section.TitleAr) ? section.TitleAr : section.TitleEn;
            var titleEn = !string.IsNullOrWhiteSpace(section.TitleEn) ? section.TitleEn : section.TitleAr;
            return op switch
            {
                ChangeOperation.Added => new EntityLogMeta(
                    ActivityResourceType.PrivacyPolicy,
                    $"إضافة بند في سياسة الخصوصية: {titleAr}",
                    $"Added Privacy Policy section: {titleEn}",
                    $"تمت إضافة بند جديد في سياسة الخصوصية",
                    $"Added section in Privacy Policy"),
                ChangeOperation.Deleted => new EntityLogMeta(
                    ActivityResourceType.PrivacyPolicy,
                    $"حذف بند من سياسة الخصوصية: {titleAr}",
                    $"Deleted Privacy Policy section: {titleEn}",
                    $"تم حذف البند من سياسة الخصوصية",
                    $"Deleted section from Privacy Policy"),
                _ => new EntityLogMeta(
                    ActivityResourceType.PrivacyPolicy,
                    $"تعديل بند في سياسة الخصوصية: {titleAr}",
                    $"Updated Privacy Policy section: {titleEn}",
                    $"تم تحديث البند في سياسة الخصوصية",
                    $"Updated section in Privacy Policy")
            };
        }

        private record EntityLogMeta(
            ActivityResourceType? ResourceType,
            string ActionAr,
            string ActionEn,
            string DetailsAr,
            string DetailsEn,
            Guid? LogUserId = null,
            string? LogUserName = null,
            string? LogUserEmail = null,
            string? UserProfilePictureUrl = null
        );
    }
}

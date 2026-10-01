using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Persistance.Seeding
{
    public static class DataSeeder
    {
        public static async Task ClearDatabaseAsync(StockExchangeDbContext context)
        {
            await context.Database.ExecuteSqlRawAsync(@"
                EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT all';

                DELETE FROM ActivityLogs;
                DELETE FROM AspNetUserRoles;
                DELETE FROM AspNetUserClaims;
                DELETE FROM AspNetUserLogins;
                DELETE FROM AspNetUserTokens;
                DELETE FROM AspNetRoleClaims;
                DELETE FROM UserRefreshTokens;
                DELETE FROM Users;
                DELETE FROM AspNetRoles;

                DELETE FROM AboutUsFeatures;
                DELETE FROM AboutUs;
                DELETE FROM HelpCenters;
                DELETE FROM HelpCenterCategories;
                DELETE FROM PrivacyPolicySections;
                DELETE FROM PrivacyPolicies;
                DELETE FROM TermsAndConditionsSections;
                DELETE FROM TermsAndConditions;
                DELETE FROM PlanFeatures;
                DELETE FROM SubscriptionPlans;
                DELETE FROM Articles;
                DELETE FROM Videos;
                DELETE FROM News;
                DELETE FROM Services;
                DELETE FROM Experts;
                DELETE FROM Home;
                DELETE FROM Countries;

                EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all';
            ");
        }

        public static async Task SeedAllAsync(StockExchangeDbContext context, UserManager<ApplicationUser>? userManager = null, RoleManager<IdentityRole<Guid>>? roleManager = null)
        {
            await CountrySeeder.SeedCountriesAsync(context);
            await UserSeeder.SeedUsersAndRolesAsync(userManager, roleManager);
            await AboutUsSeeder.SeedAboutUsAsync(context);
            await AboutUsFeatureSeeder.SeedFeaturesAsync(context);
            await HelpCenterCategorySeeder.SeedCategoriesAsync(context);
            await HelpCenterSeeder.SeedHelpCenterAsync(context);
            await PrivacyPolicySeeder.SeedPrivacyPolicyAsync(context);
            await TermsAndConditionsSeeder.SeedTermsAndConditionsAsync(context);
            await VideoSeeder.SeedVideosAsync(context);
            await ArticleSeeder.SeedArticlesAsync(context);
            await HomeSeeder.SeedHomeAsync(context);
            await ActivityLogSeeder.SeedActivityLogsAsync(context);

        }
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Application.Common.Interfaces
{
    public interface IStockExchangeDbContext : IAsyncDisposable
    {
        DbSet<IdentityUserRole<Guid>> UserRoles { get; }
        DbSet<IdentityRole<Guid>> Roles { get; }
        DbSet<UserRefreshToken> UserRefreshTokens { get; }
        DbSet<Country> Countries { get; }
        DbSet<AboutUs> AboutUs { get; }
        DbSet<AboutUsFeature> AboutUsFeatures { get; }
        DbSet<HelpCenter> HelpCenters { get; }
        DbSet<HelpCenterCategory> HelpCenterCategories { get; }
        DbSet<PrivacyPolicy> PrivacyPolicies { get; }
        DbSet<PrivacyPolicySection> PrivacyPolicySections { get; }
        DbSet<TermsAndConditions> TermsAndConditions { get; }
        DbSet<TermsAndConditionsSection> TermsAndConditionsSections { get; }
        DbSet<Home> Homes { get; }
        DbSet<News> News { get; }
        DbSet<Service> Services { get; }
        DbSet<Article> Articles { get; }
        DbSet<ArticleCategory> ArticleCategories { get; }
        DbSet<Video> Videos { get; }
        DbSet<VideoCategory> VideoCategories { get; }
        DbSet<SubscriptionPlan> SubscriptionPlans { get; }
        DbSet<PlanFeature> PlanFeatures { get; }
        DbSet<Expert> Experts { get; }
        DbSet<ApplicationUser> Users { get; }
        DbSet<ActivityLog> ActivityLogs { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

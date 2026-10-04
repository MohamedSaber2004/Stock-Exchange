using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Domain.Common;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Persistance.Services;

namespace Stock_Exchange.Persistance
{
    public class StockExchangeDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid,
        IdentityUserClaim<Guid>, IdentityUserRole<Guid>, IdentityUserLogin<Guid>,
        IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>, IStockExchangeDbContext
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IActivityLogGenerator _activityLogGenerator;

        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<AboutUs> AboutUs { get; set; }
        public DbSet<AboutUsFeature> AboutUsFeatures { get; set; }
        public DbSet<HelpCenter> HelpCenters { get; set; }
        public DbSet<HelpCenterCategory> HelpCenterCategories { get; set; }
        public DbSet<PrivacyPolicy> PrivacyPolicies { get; set; }
        public DbSet<PrivacyPolicySection> PrivacyPolicySections { get; set; }
        public DbSet<TermsAndConditions> TermsAndConditions { get; set; }
        public DbSet<TermsAndConditionsSection> TermsAndConditionsSections { get; set; }
        public DbSet<Home> Homes { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<ArticleCategory> ArticleCategories { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<VideoCategory> VideoCategories { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<PlanFeature> PlanFeatures { get; set; }
        public DbSet<Expert> Experts { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }

        public StockExchangeDbContext(
            ICurrentUserService currentUserService,
            DbContextOptions<StockExchangeDbContext> options,
            IActivityLogGenerator? activityLogGenerator = null)
            : base(options)
        {
            _currentUserService = currentUserService;
            _activityLogGenerator = activityLogGenerator ?? new ActivityLogGenerator(currentUserService);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(StockExchangeDbContext).Assembly,
                type => type.Namespace is not null && type.Namespace.EndsWith("Configurations"));

            var dateTimeConverter = new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
                v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v.ToUniversalTime(), DateTimeKind.Utc),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            var nullableDateTimeConverter = new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime?, DateTime?>(
                v => v.HasValue ? (v.Value.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v.Value.ToUniversalTime(), DateTimeKind.Utc)) : v,
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(dateTimeConverter);
                    }
                    else if (property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(nullableDateTimeConverter);
                    }
                }
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var userId = _currentUserService?.UserId.ToString() ?? "System";

            // 1. Automatically generate activity logs before entity states are modified
            var activityLogs = _activityLogGenerator.GenerateActivityLogs(ChangeTracker);

            // 2. BaseEntity auditing & soft-delete transformation
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.Entity is ActivityLog activityLog)
                {
                    if (entry.State == EntityState.Added && (activityLog.CreatedAt == default || activityLog.CreatedAt == DateTime.MinValue))
                    {
                        activityLog.MarkAsCreated(userId, DateTime.UtcNow);
                    }
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.MarkAsCreated(userId);
                        break;
                    case EntityState.Modified:
                        entry.Entity.MarkAsUpdated(userId);
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.Entity.MarkAsDeleted(userId);
                        break;
                }
            }

            // 3. ApplicationUser auditing & soft-delete transformation
            foreach (var entry in ChangeTracker.Entries().Where(e => e.Entity is ApplicationUser && e.State != EntityState.Detached))
            {
                var user = (ApplicationUser)entry.Entity;
                switch (entry.State)
                {
                    case EntityState.Added:
                        user.CreatedAt = DateTime.UtcNow;
                        user.CreatedBy = userId;
                        user.IsActive = true;
                        break;
                    case EntityState.Modified:
                        user.UpdatedAt = DateTime.UtcNow;
                        user.UpdatedBy = userId;
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        user.IsDeleted = true;
                        user.DeletedAt = DateTime.UtcNow;
                        user.DeletedBy = userId;
                        user.IsActive = false;
                        break;
                }
            }

            // 4. Attach generated activity logs so they are saved atomically in the same transaction
            if (activityLogs.Count > 0)
            {
                foreach (var log in activityLogs)
                {
                    log.MarkAsCreated(userId);
                    ActivityLogs.Add(log);
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}


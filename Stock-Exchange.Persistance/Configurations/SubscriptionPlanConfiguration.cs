using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Persistance.Configurations
{
    public class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
    {
        public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
        {
            builder.ToTable("SubscriptionPlans");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.NameEn)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(p => p.NameAr)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(p => p.PriceEgp)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(p => p.Period)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(p => p.IsHighlighted)
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(p => p.DisplayOrder)
                .IsRequired();

            builder.Property(p => p.IsActive)
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(p => p.IsDeleted)
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(p => p.CreatedBy)
                .IsRequired(false);

            builder.Property(p => p.UpdatedBy)
                .IsRequired(false);

            builder.Property(p => p.DeletedBy)
                .IsRequired(false);

            builder.Property(p => p.CreatedAt)
                .IsRequired();

            builder.Property(p => p.UpdatedAt)
                .IsRequired(false);

            builder.Property(p => p.DeletedAt)
                .IsRequired(false);

            builder.Property(p => p.Version)
                .IsRowVersion();

            builder.HasIndex(p => p.IsDeleted);

            builder.HasMany(p => p.Features)
                .WithOne(f => f.Plan)
                .HasForeignKey(f => f.PlanId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(p => !p.IsDeleted);
        }
    }
}

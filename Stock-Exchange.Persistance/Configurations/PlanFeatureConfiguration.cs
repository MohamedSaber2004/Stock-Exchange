using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Persistance.Configurations
{
    public class PlanFeatureConfiguration : IEntityTypeConfiguration<PlanFeature>
    {
        public void Configure(EntityTypeBuilder<PlanFeature> builder)
        {
            builder.ToTable("PlanFeatures");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.PlanId)
                .IsRequired();

            builder.Property(f => f.TextEn)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(f => f.TextAr)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(f => f.DisplayOrder)
                .IsRequired();

            builder.Property(f => f.IsActive)
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(f => f.IsDeleted)
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(f => f.CreatedBy)
                .IsRequired(false);

            builder.Property(f => f.UpdatedBy)
                .IsRequired(false);

            builder.Property(f => f.DeletedBy)
                .IsRequired(false);

            builder.Property(f => f.CreatedAt)
                .IsRequired();

            builder.Property(f => f.UpdatedAt)
                .IsRequired(false);

            builder.Property(f => f.DeletedAt)
                .IsRequired(false);

            builder.Property(f => f.Version)
                .IsRowVersion();

            builder.HasIndex(f => f.PlanId);
            builder.HasIndex(f => f.IsDeleted);

            builder.HasQueryFilter(f => !f.IsDeleted);
        }
    }
}

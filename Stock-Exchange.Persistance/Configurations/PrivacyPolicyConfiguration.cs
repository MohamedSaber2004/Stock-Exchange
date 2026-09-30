using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Persistance.Configurations
{
    public class PrivacyPolicyConfiguration : IEntityTypeConfiguration<PrivacyPolicy>
    {
        public void Configure(EntityTypeBuilder<PrivacyPolicy> builder)
        {
            builder.ToTable("PrivacyPolicies");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.TitleEn)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(p => p.TitleAr)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(p => p.DescriptionEn)
                .HasMaxLength(4000)
                .IsRequired(false);

            builder.Property(p => p.DescriptionAr)
                .HasMaxLength(4000)
                .IsRequired(false);

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

            builder.HasQueryFilter(p => !p.IsDeleted);
        }
    }
}

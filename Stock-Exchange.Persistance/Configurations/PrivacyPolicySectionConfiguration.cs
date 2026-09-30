using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Persistance.Configurations
{
    public class PrivacyPolicySectionConfiguration : IEntityTypeConfiguration<PrivacyPolicySection>
    {
        public void Configure(EntityTypeBuilder<PrivacyPolicySection> builder)
        {
            builder.ToTable("PrivacyPolicySections");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.TitleEn)
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(s => s.TitleAr)
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(s => s.ContentEn)
                .HasMaxLength(8000)
                .IsRequired();

            builder.Property(s => s.ContentAr)
                .HasMaxLength(8000)
                .IsRequired();

            builder.Property(s => s.DisplayOrder)
                .IsRequired();

            builder.Property(s => s.IsActive)
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(s => s.IsDeleted)
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(s => s.CreatedBy)
                .IsRequired(false);

            builder.Property(s => s.UpdatedBy)
                .IsRequired(false);

            builder.Property(s => s.DeletedBy)
                .IsRequired(false);

            builder.Property(s => s.CreatedAt)
                .IsRequired();

            builder.Property(s => s.UpdatedAt)
                .IsRequired(false);

            builder.Property(s => s.DeletedAt)
                .IsRequired(false);

            builder.Property(s => s.Version)
                .IsRowVersion();

            builder.HasIndex(s => s.PrivacyPolicyId);
            builder.HasIndex(s => s.IsDeleted);

            builder.HasOne(s => s.PrivacyPolicy)
                .WithMany(p => p.Sections)
                .HasForeignKey(s => s.PrivacyPolicyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(s => !s.IsDeleted);
        }
    }
}

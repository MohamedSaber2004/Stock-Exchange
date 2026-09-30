using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Persistance.Configurations
{
    public class TermsAndConditionsSectionConfiguration : IEntityTypeConfiguration<TermsAndConditionsSection>
    {
        public void Configure(EntityTypeBuilder<TermsAndConditionsSection> builder)
        {
            builder.ToTable("TermsAndConditionsSections");

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

            builder.HasIndex(s => s.TermsAndConditionsId);
            builder.HasIndex(s => s.IsDeleted);

            builder.HasOne(s => s.TermsAndConditions)
                .WithMany(t => t.Sections)
                .HasForeignKey(s => s.TermsAndConditionsId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(s => !s.IsDeleted);
        }
    }
}

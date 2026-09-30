using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Persistance.Configurations
{
    public class TermsAndConditionsConfiguration : IEntityTypeConfiguration<TermsAndConditions>
    {
        public void Configure(EntityTypeBuilder<TermsAndConditions> builder)
        {
            builder.ToTable("TermsAndConditions");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.TitleEn)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(t => t.TitleAr)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(t => t.DescriptionEn)
                .HasMaxLength(4000)
                .IsRequired(false);

            builder.Property(t => t.DescriptionAr)
                .HasMaxLength(4000)
                .IsRequired(false);

            builder.Property(t => t.IsActive)
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(t => t.IsDeleted)
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(t => t.CreatedBy)
                .IsRequired(false);

            builder.Property(t => t.UpdatedBy)
                .IsRequired(false);

            builder.Property(t => t.DeletedBy)
                .IsRequired(false);

            builder.Property(t => t.CreatedAt)
                .IsRequired();

            builder.Property(t => t.UpdatedAt)
                .IsRequired(false);

            builder.Property(t => t.DeletedAt)
                .IsRequired(false);

            builder.Property(t => t.Version)
                .IsRowVersion();

            builder.HasIndex(t => t.IsDeleted);

            builder.HasQueryFilter(t => !t.IsDeleted);
        }
    }
}

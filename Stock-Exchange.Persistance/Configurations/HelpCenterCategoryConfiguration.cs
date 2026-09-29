using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Persistance.Configurations
{
    public class HelpCenterCategoryConfiguration : IEntityTypeConfiguration<HelpCenterCategory>
    {
        public void Configure(EntityTypeBuilder<HelpCenterCategory> builder)
        {
            builder.ToTable("HelpCenterCategories");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.TitleEn)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(c => c.TitleAr)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(c => c.DisplayOrder)
                .IsRequired();

            builder.Property(c => c.IsActive)
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(c => c.IsDeleted)
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(c => c.CreatedBy)
                .IsRequired(false);

            builder.Property(c => c.UpdatedBy)
                .IsRequired(false);

            builder.Property(c => c.DeletedBy)
                .IsRequired(false);

            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.Property(c => c.UpdatedAt)
                .IsRequired(false);

            builder.Property(c => c.DeletedAt)
                .IsRequired(false);

            builder.Property(c => c.Version)
                .IsRowVersion();

            builder.HasIndex(c => c.IsDeleted);

            builder.HasQueryFilter(c => !c.IsDeleted);
        }
    }
}

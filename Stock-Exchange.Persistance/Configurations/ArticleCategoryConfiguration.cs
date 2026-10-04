using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Persistance.Configurations
{
    public class ArticleCategoryConfiguration : IEntityTypeConfiguration<ArticleCategory>
    {
        public void Configure(EntityTypeBuilder<ArticleCategory> builder)
        {
            builder.ToTable("ArticleCategories");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.CategoryArName)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(c => c.CategoryEnName)
                .HasMaxLength(150)
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

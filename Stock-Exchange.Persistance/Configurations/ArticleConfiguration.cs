using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Persistance.Configurations
{
    public class ArticleConfiguration : IEntityTypeConfiguration<Article>
    {
        public void Configure(EntityTypeBuilder<Article> builder)
        {
            builder.ToTable("Articles");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.TitleEn)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(a => a.TitleAr)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(a => a.ExcerptEn)
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(a => a.ExcerptAr)
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(a => a.ImageUrl)
                .HasMaxLength(1024)
                .IsRequired(false);

            builder.Property(a => a.AuthorName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(a => a.ReadMinutes)
                .HasDefaultValue(5)
                .IsRequired();

            builder.Property(a => a.PublishedAt)
                .IsRequired();

            builder.Property(a => a.IsFeaturedOnHome)
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(a => a.DisplayOrder)
                .IsRequired();

            builder.Property(a => a.IsActive)
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(a => a.IsDeleted)
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(a => a.CreatedBy)
                .IsRequired(false);

            builder.Property(a => a.UpdatedBy)
                .IsRequired(false);

            builder.Property(a => a.DeletedBy)
                .IsRequired(false);

            builder.Property(a => a.CreatedAt)
                .IsRequired();

            builder.Property(a => a.UpdatedAt)
                .IsRequired(false);

            builder.Property(a => a.DeletedAt)
                .IsRequired(false);

            builder.Property(a => a.Version)
                .IsRowVersion();

            builder.HasIndex(a => a.IsDeleted);
            builder.HasIndex(a => new { a.IsDeleted, a.IsFeaturedOnHome, a.PublishedAt });

            builder.HasQueryFilter(a => !a.IsDeleted);
        }
    }
}

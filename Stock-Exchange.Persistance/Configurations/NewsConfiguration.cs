using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Persistance.Configurations
{
    public class NewsConfiguration : IEntityTypeConfiguration<News>
    {
        public void Configure(EntityTypeBuilder<News> builder)
        {
            builder.ToTable("News");

            builder.HasKey(n => n.Id);

            builder.Property(n => n.TitleEn)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(n => n.TitleAr)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(n => n.SummaryEn)
                .HasMaxLength(4000)
                .IsRequired();

            builder.Property(n => n.SummaryAr)
                .HasMaxLength(4000)
                .IsRequired();

            builder.Property(n => n.ContentEn)
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

            builder.Property(n => n.ContentAr)
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

            builder.Property(n => n.ImageUrl)
                .HasMaxLength(1024)
                .IsRequired(false);

            builder.Property(n => n.CategoryEn)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(n => n.CategoryAr)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(n => n.PublishedAt)
                .IsRequired();

            builder.Property(n => n.DisplayOrder)
                .IsRequired();

            builder.Property(n => n.IsFeaturedOnHome)
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(n => n.IsActive)
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(n => n.IsDeleted)
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(n => n.CreatedBy)
                .IsRequired(false);

            builder.Property(n => n.UpdatedBy)
                .IsRequired(false);

            builder.Property(n => n.DeletedBy)
                .IsRequired(false);

            builder.Property(n => n.CreatedAt)
                .IsRequired();

            builder.Property(n => n.UpdatedAt)
                .IsRequired(false);

            builder.Property(n => n.DeletedAt)
                .IsRequired(false);

            builder.Property(n => n.Version)
                .IsRowVersion();

            builder.HasIndex(n => n.IsDeleted);
            builder.HasIndex(n => new { n.IsDeleted, n.IsFeaturedOnHome, n.PublishedAt });

            builder.HasQueryFilter(n => !n.IsDeleted);
        }
    }
}

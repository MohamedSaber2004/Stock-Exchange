using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Persistance.Configurations
{
    public class VideoConfiguration : IEntityTypeConfiguration<Video>
    {
        public void Configure(EntityTypeBuilder<Video> builder)
        {
            builder.ToTable("Videos");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.TitleEn)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(v => v.TitleAr)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(v => v.DescriptionEn)
                .HasMaxLength(4000)
                .IsRequired(false);

            builder.Property(v => v.DescriptionAr)
                .HasMaxLength(4000)
                .IsRequired(false);

            builder.Property(v => v.ThumbnailUrl)
                .HasMaxLength(1024)
                .IsRequired(false);

            builder.Property(v => v.VideoUrl)
                .HasMaxLength(1024)
                .IsRequired(false);

            builder.Property(v => v.DurationSeconds)
                .IsRequired();

            builder.Property(v => v.InstructorName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(v => v.CategoryEn)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(v => v.CategoryAr)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(v => v.IsPreviewable)
                .HasDefaultValue(true)
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(v => v.IsFeaturedOnHome)
                .HasDefaultValue(true)
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(v => v.DisplayOrder)
                .IsRequired();

            builder.Property(v => v.IsActive)
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(v => v.IsDeleted)
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(v => v.CreatedBy)
                .IsRequired(false);

            builder.Property(v => v.UpdatedBy)
                .IsRequired(false);

            builder.Property(v => v.DeletedBy)
                .IsRequired(false);

            builder.Property(v => v.CreatedAt)
                .IsRequired();

            builder.Property(v => v.UpdatedAt)
                .IsRequired(false);

            builder.Property(v => v.DeletedAt)
                .IsRequired(false);

            builder.Property(v => v.Version)
                .IsRowVersion();

            builder.HasIndex(v => v.CategoryId);
            builder.HasIndex(v => v.IsDeleted);
            builder.HasIndex(v => new { v.IsDeleted, v.IsFeaturedOnHome });

            builder.HasOne(v => v.Category)
                .WithMany(c => c.Videos)
                .HasForeignKey(v => v.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasQueryFilter(v => !v.IsDeleted);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Persistance.Configurations
{
    public class ExpertConfiguration : IEntityTypeConfiguration<Expert>
    {
        public void Configure(EntityTypeBuilder<Expert> builder)
        {
            builder.ToTable("Experts");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.FullNameEn)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(e => e.FullNameAr)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(e => e.TitleEn)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(e => e.TitleAr)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(e => e.AvatarUrl)
                .HasMaxLength(1024)
                .IsRequired(false);

            builder.Property(e => e.DisplayOrder)
                .IsRequired();

            builder.Property(e => e.IsFeaturedOnHome)
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(e => e.CreatedBy)
                .IsRequired(false);

            builder.Property(e => e.UpdatedBy)
                .IsRequired(false);

            builder.Property(e => e.DeletedBy)
                .IsRequired(false);

            builder.Property(e => e.CreatedAt)
                .IsRequired();

            builder.Property(e => e.UpdatedAt)
                .IsRequired(false);

            builder.Property(e => e.DeletedAt)
                .IsRequired(false);

            builder.Property(e => e.Version)
                .IsRowVersion();

            builder.HasIndex(e => e.IsDeleted);
            builder.HasIndex(e => new { e.IsDeleted, e.IsFeaturedOnHome });

            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}

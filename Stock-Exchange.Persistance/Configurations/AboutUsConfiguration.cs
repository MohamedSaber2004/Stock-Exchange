using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Persistance.Configurations
{
    public class AboutUsConfiguration : IEntityTypeConfiguration<AboutUs>
    {
        public void Configure(EntityTypeBuilder<AboutUs> builder)
        {
            builder.ToTable("AboutUs");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.StoryEn)
                .HasMaxLength(4000)
                .IsRequired();

            builder.Property(a => a.StoryAr)
                .HasMaxLength(4000)
                .IsRequired();

            builder.Property(a => a.MissionEn)
                .HasMaxLength(4000)
                .IsRequired();

            builder.Property(a => a.MissionAr)
                .HasMaxLength(4000)
                .IsRequired();

            builder.Property(a => a.VisionEn)
                .HasMaxLength(4000)
                .IsRequired();

            builder.Property(a => a.VisionAr)
                .HasMaxLength(4000)
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

            builder.HasQueryFilter(a => !a.IsDeleted);
        }
    }
}

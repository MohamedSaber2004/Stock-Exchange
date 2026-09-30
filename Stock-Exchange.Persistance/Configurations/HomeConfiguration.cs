using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Persistance.Configurations
{
    public class HomeConfiguration : IEntityTypeConfiguration<Home>
    {
        public void Configure(EntityTypeBuilder<Home> builder)
        {
            builder.ToTable("Home");

            builder.HasKey(h => h.Id);

            builder.Property(h => h.HeroTitleEn)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(h => h.HeroTitleAr)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(h => h.HeroSubtitleEn)
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(h => h.HeroSubtitleAr)
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(h => h.HeroImageUrl)
                .HasMaxLength(1024)
                .IsRequired(false);

            builder.Property(h => h.IsActive)
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(h => h.IsDeleted)
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(h => h.CreatedBy)
                .IsRequired(false);

            builder.Property(h => h.UpdatedBy)
                .IsRequired(false);

            builder.Property(h => h.DeletedBy)
                .IsRequired(false);

            builder.Property(h => h.CreatedAt)
                .IsRequired();

            builder.Property(h => h.UpdatedAt)
                .IsRequired(false);

            builder.Property(h => h.DeletedAt)
                .IsRequired(false);

            builder.Property(h => h.Version)
                .IsRowVersion();

            builder.HasIndex(h => h.IsDeleted);

            builder.HasQueryFilter(h => !h.IsDeleted);
        }
    }
}

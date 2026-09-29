using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Persistance.Configurations
{
    public class AboutUsFeatureConfiguration : IEntityTypeConfiguration<AboutUsFeature>
    {
        public void Configure(EntityTypeBuilder<AboutUsFeature> builder)
        {
            builder.ToTable("AboutUsFeatures");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.TitleEn)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(f => f.TitleAr)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(f => f.DescriptionEn)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(f => f.DescriptionAr)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(f => f.Category)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(f => f.DisplayOrder)
                .IsRequired();

            builder.Property(f => f.IsActive)
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(f => f.IsDeleted)
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(f => f.CreatedBy)
                .IsRequired(false);

            builder.Property(f => f.UpdatedBy)
                .IsRequired(false);

            builder.Property(f => f.DeletedBy)
                .IsRequired(false);

            builder.Property(f => f.CreatedAt)
                .IsRequired();

            builder.Property(f => f.UpdatedAt)
                .IsRequired(false);

            builder.Property(f => f.DeletedAt)
                .IsRequired(false);

            builder.Property(f => f.Version)
                .IsRowVersion();

            builder.HasIndex(f => f.AboutUsId);
            builder.HasIndex(f => f.IsDeleted);

            builder.HasOne(f => f.AboutUs)
                .WithMany(a => a.Features)
                .HasForeignKey(f => f.AboutUsId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(f => !f.IsDeleted);
        }
    }
}

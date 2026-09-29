using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Persistance.Configurations
{
    public class HelpCenterConfiguration : IEntityTypeConfiguration<HelpCenter>
    {
        public void Configure(EntityTypeBuilder<HelpCenter> builder)
        {
            builder.ToTable("HelpCenters");

            builder.HasKey(h => h.Id);

            builder.Property(h => h.TitleEn)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(h => h.TitleAr)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(h => h.ContentEn)
                .HasMaxLength(4000)
                .IsRequired();

            builder.Property(h => h.ContentAr)
                .HasMaxLength(4000)
                .IsRequired();

            builder.Property(h => h.DisplayOrder)
                .IsRequired();

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

            builder.HasIndex(h => h.CategoryId);
            builder.HasIndex(h => h.IsDeleted);

            builder.HasOne(h => h.Category)
                .WithMany(c => c.HelpCenters)
                .HasForeignKey(h => h.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasQueryFilter(h => !h.IsDeleted);
        }
    }
}

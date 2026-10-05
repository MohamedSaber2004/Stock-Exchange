using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Persistance.Configurations
{
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.ToTable("Services");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.TitleEn)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(s => s.TitleAr)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(s => s.DescriptionEn)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(s => s.DescriptionAr)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(s => s.ContentEn)
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

            builder.Property(s => s.ContentAr)
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

            builder.Property(s => s.IconName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(s => s.ImageUrl)
                .HasMaxLength(1024)
                .IsRequired(false);

            builder.Property(s => s.LinkRoute)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(s => s.DisplayOrder)
                .IsRequired();

            builder.Property(s => s.IsActive)
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(s => s.IsDeleted)
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(s => s.CreatedBy)
                .IsRequired(false);

            builder.Property(s => s.UpdatedBy)
                .IsRequired(false);

            builder.Property(s => s.DeletedBy)
                .IsRequired(false);

            builder.Property(s => s.CreatedAt)
                .IsRequired();

            builder.Property(s => s.UpdatedAt)
                .IsRequired(false);

            builder.Property(s => s.DeletedAt)
                .IsRequired(false);

            builder.Property(s => s.Version)
                .IsRowVersion();

            builder.HasIndex(s => s.IsDeleted);

            builder.HasQueryFilter(s => !s.IsDeleted);
        }
    }
}

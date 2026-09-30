using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Persistance.Configurations
{
    public class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>
    {
        public void Configure(EntityTypeBuilder<ActivityLog> builder)
        {
            builder.ToTable("ActivityLogs");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.FormattedId)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(a => a.UserName)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(a => a.UserEmail)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(a => a.UserProfilePictureUrl)
                .HasMaxLength(1024)
                .IsRequired(false);

            builder.Property(a => a.Action)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(a => a.ActionAr)
                .HasMaxLength(1000)
                .IsRequired(false);

            builder.Property(a => a.ActionEn)
                .HasMaxLength(1000)
                .IsRequired(false);

            builder.Property(a => a.ResourceType)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(a => a.IpAddress)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(a => a.Device)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(a => a.Details)
                .HasMaxLength(4000)
                .IsRequired(false);

            builder.Property(a => a.DetailsAr)
                .HasMaxLength(4000)
                .IsRequired(false);

            builder.Property(a => a.DetailsEn)
                .HasMaxLength(4000)
                .IsRequired(false);

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

            builder.HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(a => a.FormattedId);
            builder.HasIndex(a => a.ResourceType);
            builder.HasIndex(a => a.CreatedAt);
            builder.HasIndex(a => a.IsDeleted);

            builder.HasQueryFilter(a => !a.IsDeleted);
        }
    }
}

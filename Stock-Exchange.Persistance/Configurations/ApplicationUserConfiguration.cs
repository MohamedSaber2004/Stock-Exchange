using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Persistance.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.ToTable("Users");

            builder.Ignore(u => u.DomainEvents);

            builder.Property(u => u.FullName)
                .IsRequired();

            builder.Property(u => u.ProfilePictureUrl)
                .IsRequired(false);

            builder.Property(u => u.PasswordResetToken)
                .IsRequired(false);

            builder.Property(u => u.PasswordResetTokenExpiry)
                .IsRequired(false);

            builder.Property(u => u.VerificationCode)
                .IsRequired(false);

            builder.Property(u => u.VerificationCodeExpiry)
                .IsRequired(false);

            builder.Property(u => u.GoogleUserId)
                .IsRequired(false);

            builder.Property(u => u.TokenVersion)
                .HasDefaultValue(0L)
                .IsRequired();

            builder.Property(u => u.Language)
                .HasConversion<string>()
                .HasDefaultValue(Language.en)
                .IsRequired();

            builder.Property(u => u.CreatedBy)
                .IsRequired(false);

            builder.Property(u => u.UpdatedBy)
                .IsRequired(false);

            builder.Property(u => u.DeletedBy)
                .IsRequired(false);

            builder.Property(u => u.CreatedAt)
                .IsRequired();

            builder.Property(u => u.UpdatedAt)
                .IsRequired(false);

            builder.Property(u => u.DeletedAt)
                .IsRequired(false);

            builder.Property(u => u.IsActive)
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(u => u.IsDeleted)
                .HasDefaultValue(false)
                .IsRequired();

            builder.HasIndex(u => u.GoogleUserId)
                .IsUnique();

            builder.HasIndex(u => u.IsDeleted);

            builder.HasQueryFilter(u => !u.IsDeleted);
        }
    }
}

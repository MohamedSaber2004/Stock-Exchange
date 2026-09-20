using Microsoft.AspNetCore.Identity;
using Stock_Exchange.Domain.Common.Base;
using Stock_Exchange.Domain.Common.Events;
using Stock_Exchange.Domain.Common.Exceptions;
using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>, IBaseEntity<Guid>
    {
        private readonly List<IDomainEvent> _domainEvents = new();

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string? UpdatedBy { get; set; }
        public string? DeletedBy { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; } = true;

        public string FullName { get; private set; } = null!;
        public string? ProfilePictureUrl { get; private set; }
        public string? PasswordResetToken { get; private set; }
        public DateTime? PasswordResetTokenExpiry { get; private set; }
        public Language Language { get; private set; } = Language.en;

        public string? VerificationCode { get; private set; }
        public DateTime? VerificationCodeExpiry { get; private set; }
        public string? GoogleUserId { get; private set; }

        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        public void MarkAsCreated(string createdBy)
        {
            CreatedAt = DateTime.UtcNow;
            CreatedBy = createdBy ?? string.Empty;
            IsActive = true;
            IsDeleted = false;
        }

        public void MarkAsUpdated(string updatedBy)
        {
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = updatedBy ?? string.Empty;
        }

        public void MarkAsDeleted(string deletedBy)
        {
            DeletedAt = DateTime.UtcNow;
            DeletedBy = deletedBy ?? string.Empty;
            IsDeleted = true;
            IsActive = false;
        }

        public void Activate()
        {
            IsActive = true;
            IsDeleted = false;
        }

        public void Deactivate()
        {
            IsActive = false;
            IsDeleted = true;
        }

        public void UpdateFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new DomainException("Users.FullNameEmpty");

            FullName = fullName.Trim();
        }

        public void UpdateProfilePicture(string? url)
        {
            ProfilePictureUrl = url;
        }

        public void ChangeLanguage(Language language)
        {
            Language = language;
        }

        public void SetPasswordResetToken(string token, DateTime expiry)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new DomainException("Users.PasswordResetTokenEmpty");

            PasswordResetToken = token;
            PasswordResetTokenExpiry = expiry;
        }

        public void SetVerificationCode(string code, DateTime expiry)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new DomainException("Users.VerificationCodeEmpty");

            VerificationCode = code;
            VerificationCodeExpiry = expiry;
        }

        public void ClearVerificationCode()
        {
            VerificationCode = null;
            VerificationCodeExpiry = null;
        }
    }
}
using Microsoft.AspNetCore.Identity;
using Stock_Exchange.Domain.Common.Base;
using Stock_Exchange.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Stock_Exchange.Domain.Entities
{
    public class ApplicationUser: IdentityUser<Guid>, IBaseEntity<Guid>
    {
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
        public string? FacebookUserId { get; private set; }
        public string? GoogleUserId { get; private set; }
    }
}

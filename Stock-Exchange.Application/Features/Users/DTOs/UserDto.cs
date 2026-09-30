using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.Users.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public UserType UserType { get; set; }
        public string UserTypeArabic => UserType == UserType.Admin ? "مدير" : "مستخدم";
        public string UserTypeEnglish => UserType == UserType.Admin ? "Admin" : "Customer";
        public string UserTypeTitle { get; set; } = string.Empty;
        public string Role => UserType.ToString();
        public bool IsActive { get; set; }
        public string StatusArabic => IsActive ? "نشط" : "غير نشط";
        public string StatusEnglish => IsActive ? "Active" : "Inactive";
        public string StatusTitle { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public Guid? CountryId { get; set; }
        public string? CountryArName { get; set; }
        public string? CountryEnName { get; set; }
        public string? CountryName { get; set; }
        public string? CountryCode { get; set; }

        public void ApplyLanguageFilter(Language language)
        {
            if (language == Language.ar)
            {
                UserTypeTitle = UserTypeArabic;
                StatusTitle = StatusArabic;
                CountryName = !string.IsNullOrEmpty(CountryArName) ? CountryArName : CountryEnName;
            }
            else
            {
                UserTypeTitle = UserTypeEnglish;
                StatusTitle = StatusEnglish;
                CountryName = !string.IsNullOrEmpty(CountryEnName) ? CountryEnName : CountryArName;
            }
        }
    }
}

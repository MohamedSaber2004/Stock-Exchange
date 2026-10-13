using System.Text.Json.Serialization;
using MediatR;
using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.Auth.Commands.UpdateUserInfo
{
    public class UpdateUserInfoCommand : IRequest<string>
    {
        private string? _profilePictureUrl;

        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public Guid? CountryId { get; set; }
        public Language? Language { get; set; }

        public string? ProfilePictureUrl
        {
            get => _profilePictureUrl;
            set
            {
                _profilePictureUrl = value;
                IsProfilePictureUrlSpecified = true;
            }
        }

        [JsonIgnore]
        public bool IsProfilePictureUrlSpecified { get; set; }

        [JsonConstructor]
        public UpdateUserInfoCommand()
        {
        }

        public UpdateUserInfoCommand(
            string? fullName = null,
            string? email = null,
            string? phoneNumber = null,
            Guid? countryId = null,
            Language? language = null,
            string? profilePictureUrl = null,
            bool isProfilePictureUrlSpecified = false)
        {
            FullName = fullName;
            Email = email;
            PhoneNumber = phoneNumber;
            CountryId = countryId;
            Language = language;
            _profilePictureUrl = profilePictureUrl;
            IsProfilePictureUrlSpecified = isProfilePictureUrlSpecified || profilePictureUrl != null;
        }
    }
}

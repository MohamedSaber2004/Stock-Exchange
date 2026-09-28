using MediatR;

namespace Stock_Exchange.Application.Features.Auth.Commands.UpdateUserInfo
{
    public class UpdateUserInfoCommand : IRequest<string>
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public Guid CountryId { get; set; }

        public UpdateUserInfoCommand()
        {
        }

        public UpdateUserInfoCommand(string fullName, string email, string phoneNumber, Guid countryId)
        {
            FullName = fullName;
            Email = email;
            PhoneNumber = phoneNumber;
            CountryId = countryId;
        }
    }
}

using MediatR;
using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.Auth.Commands.UpdateUserInfo
{
    public class UpdateUserInfoCommand : IRequest<string>
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public Guid? CountryId { get; set; }
        public Language? Language { get; set; }

        public UpdateUserInfoCommand()
        {
        }

        public UpdateUserInfoCommand(
            string? fullName = null,
            string? email = null,
            string? phoneNumber = null,
            Guid? countryId = null,
            Language? language = null)
        {
            FullName = fullName;
            Email = email;
            PhoneNumber = phoneNumber;
            CountryId = countryId;
            Language = language;
        }
    }
}

using MediatR;
using Stock_Exchange.Application.Features.Auth.DTOs;

namespace Stock_Exchange.Application.Features.Auth.Commands.Register
{
    public class SignupCommand : IRequest<AuthResponseDto>
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public Guid CountryId { get; set; }

        public SignupCommand()
        {
        }

        public SignupCommand(
            string fullName,
            string email,
            string password,
            string confirmPassword,
            string phoneNumber,
            Guid countryId)
        {
            FullName = fullName;
            Email = email;
            Password = password;
            ConfirmPassword = confirmPassword;
            PhoneNumber = phoneNumber;
            CountryId = countryId;
        }
    }
}

using MediatR;
using Stock_Exchange.Application.Features.Auth.DTOs;

namespace Stock_Exchange.Application.Features.Auth.Commands.Login
{
    public class LoginCommand : IRequest<AuthResponseDto>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public LoginCommand()
        {
        }

        public LoginCommand(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}

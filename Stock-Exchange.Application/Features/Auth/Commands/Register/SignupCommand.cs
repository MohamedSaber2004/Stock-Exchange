using MediatR;
using Stock_Exchange.Application.Features.Auth.DTOs;

namespace Stock_Exchange.Application.Features.Auth.Commands.Register
{
    public record SignupCommand(
        string FullName,
        string Email,
        string Password,
        string ConfirmPassword,
        string PhoneNumber) : IRequest<AuthResponseDto>;
}

using MediatR;
using Stock_Exchange.Application.Features.Auth.DTOs;

namespace Stock_Exchange.Application.Features.Auth.Commands.Login
{
    public record LoginCommand(
    string Email,
    string Password) : IRequest<AuthResponseDto>;
}

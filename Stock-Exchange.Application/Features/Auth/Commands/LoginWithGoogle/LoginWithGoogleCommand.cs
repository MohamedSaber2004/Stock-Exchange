using MediatR;
using Stock_Exchange.Application.Features.Auth.DTOs;

namespace Stock_Exchange.Application.Features.Auth.Commands.LoginWithGoogle
{
    public record LoginWithGoogleCommand(
        string IdToken) : IRequest<AuthResponseDto>;
}

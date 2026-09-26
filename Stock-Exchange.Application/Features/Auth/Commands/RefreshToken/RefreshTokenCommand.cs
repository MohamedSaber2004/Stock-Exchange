using MediatR;
using Stock_Exchange.Application.Features.Auth.DTOs;

namespace Stock_Exchange.Application.Features.Auth.Commands.RefreshToken
{
    public record RefreshTokenCommand : IRequest<RefreshTokenResponseDto>
    {
        public string RefreshToken { get; set; } = null!;
    }
}

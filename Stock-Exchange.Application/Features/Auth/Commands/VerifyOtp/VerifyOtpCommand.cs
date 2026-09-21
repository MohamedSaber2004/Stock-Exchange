using MediatR;

namespace Stock_Exchange.Application.Features.Auth.Commands.VerifyOtp
{
    public record VerifyOtpCommand : IRequest<string>
    {
        public string Email { get; set; } = null!;
        public string OtpCode { get; set; } = null!;
    }
}

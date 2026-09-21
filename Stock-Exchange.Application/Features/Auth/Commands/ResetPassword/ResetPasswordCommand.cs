using MediatR;

namespace Stock_Exchange.Application.Features.Auth.Commands.ResetPassword
{
    public record ResetPasswordCommand : IRequest<bool>
    {
        public string Email { get; set; } = null!;
        public string OtpCode { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
    }
}

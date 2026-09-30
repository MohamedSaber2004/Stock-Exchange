using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Users.DTOs;
using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.Users.Commands.AddUser
{
    public record AddUserCommand : IRequest<Result<UserDto>>
    {
        public string FullName { get; init; } = null!;
        public string Email { get; init; } = null!;
        public string Password { get; init; } = null!;
        public string ConfirmPassword { get; init; } = null!;
        public string? PhoneNumber { get; init; }
        public Guid? CountryId { get; init; }
        public UserType UserType { get; init; } = UserType.Customer;
        public bool IsActive { get; init; } = true;
    }
}

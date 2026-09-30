using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Users.DTOs;
using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.Users.Commands.UpdateUser
{
    public record UpdateUserCommand : IRequest<Result<UserDto>>
    {
        public Guid Id { get; init; }
        public string FullName { get; init; } = null!;
        public string Email { get; init; } = null!;
        public string? PhoneNumber { get; init; }
        public Guid? CountryId { get; init; }
        public UserType UserType { get; init; }
        public bool IsActive { get; init; }
    }
}

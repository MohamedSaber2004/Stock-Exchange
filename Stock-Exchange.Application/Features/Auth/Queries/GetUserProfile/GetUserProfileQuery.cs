using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Auth.DTOs;

namespace Stock_Exchange.Application.Features.Auth.Queries.GetUserProfile
{
    public class GetUserProfileQuery : IRequest<Result<UserProfileDto>>
    {
    }
}

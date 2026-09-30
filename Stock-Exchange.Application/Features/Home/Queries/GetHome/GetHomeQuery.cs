using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Home.DTOs;

namespace Stock_Exchange.Application.Features.Home.Queries.GetHome
{
    public class GetHomeQuery : IRequest<Result<HomeDto>>
    {
    }
}

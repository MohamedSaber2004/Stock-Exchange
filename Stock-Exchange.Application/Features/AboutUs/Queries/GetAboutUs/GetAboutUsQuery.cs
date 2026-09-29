using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.AboutUs.DTOs;

namespace Stock_Exchange.Application.Features.AboutUs.Queries.GetAboutUs
{
    public class GetAboutUsQuery : IRequest<Result<AboutUsDto>>
    {
    }
}

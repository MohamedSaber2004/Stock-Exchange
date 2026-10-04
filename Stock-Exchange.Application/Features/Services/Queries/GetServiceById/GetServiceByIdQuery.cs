using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Services.DTOs;

namespace Stock_Exchange.Application.Features.Services.Queries.GetServiceById
{
    public record GetServiceByIdQuery(Guid Id, bool? ApplyLanguageFilter = null) : IRequest<Result<ServiceDto>>;
}

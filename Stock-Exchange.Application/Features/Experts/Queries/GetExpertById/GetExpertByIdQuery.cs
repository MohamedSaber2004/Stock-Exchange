using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Experts.DTOs;

namespace Stock_Exchange.Application.Features.Experts.Queries.GetExpertById
{
    public record GetExpertByIdQuery(Guid Id, bool? ApplyLanguageFilter = null) : IRequest<Result<ExpertDto>>;
}

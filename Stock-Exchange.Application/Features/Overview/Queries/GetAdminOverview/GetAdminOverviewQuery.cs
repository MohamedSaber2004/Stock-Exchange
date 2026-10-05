using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Overview.DTOs;

namespace Stock_Exchange.Application.Features.Overview.Queries.GetAdminOverview;

public record GetAdminOverviewQuery : IRequest<Result<AdminOverviewDto>>;

using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Search.DTOs;

namespace Stock_Exchange.Application.Features.Search.Queries.GlobalSearch;

public record GlobalSearchQuery(string Query, int Limit = 5) : IRequest<Result<GlobalSearchResultDto>>;
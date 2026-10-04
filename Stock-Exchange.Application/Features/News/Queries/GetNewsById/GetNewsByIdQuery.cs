using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.News.DTOs;

namespace Stock_Exchange.Application.Features.News.Queries.GetNewsById
{
    public record GetNewsByIdQuery(Guid Id, bool? ApplyLanguageFilter = null) : IRequest<Result<NewsDto>>;
}

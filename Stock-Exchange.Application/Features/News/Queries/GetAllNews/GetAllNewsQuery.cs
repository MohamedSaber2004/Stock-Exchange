using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.News.DTOs;

namespace Stock_Exchange.Application.Features.News.Queries.GetAllNews
{
    public class GetAllNewsQuery : IRequest<Result<PagginatedResult<NewsDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public string? Category { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsFeaturedOnHome { get; set; }
        public bool ApplyLanguageFilter { get; set; } = true;
    }
}

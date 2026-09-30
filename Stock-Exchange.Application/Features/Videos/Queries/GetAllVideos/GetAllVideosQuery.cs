using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Videos.DTOs;

namespace Stock_Exchange.Application.Features.Videos.Queries.GetAllVideos
{
    public class GetAllVideosQuery : IRequest<Result<PagginatedResult<VideoDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public string? Category { get; set; }
        public bool? IsActive { get; set; }
        public bool ApplyLanguageFilter { get; set; } = true;

        public GetAllVideosQuery()
        {
        }

        public GetAllVideosQuery(int pageNumber, int pageSize, string? search = null, string? category = null, bool? isActive = null, bool applyLanguageFilter = true)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            Search = search;
            Category = category;
            IsActive = isActive;
            ApplyLanguageFilter = applyLanguageFilter;
        }
    }
}

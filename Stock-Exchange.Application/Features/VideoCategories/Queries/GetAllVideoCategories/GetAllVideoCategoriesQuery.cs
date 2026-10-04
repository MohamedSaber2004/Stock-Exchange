using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.VideoCategories.DTOs;

namespace Stock_Exchange.Application.Features.VideoCategories.Queries.GetAllVideoCategories
{
    public class GetAllVideoCategoriesQuery : IRequest<Result<List<VideoCategoryDto>>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
        public bool? ApplyLanguageFilter { get; set; }

        public GetAllVideoCategoriesQuery()
        {
        }

        public GetAllVideoCategoriesQuery(string? search, bool? isActive = null, bool? applyLanguageFilter = null)
        {
            Search = search;
            IsActive = isActive;
            ApplyLanguageFilter = applyLanguageFilter;
        }
    }
}

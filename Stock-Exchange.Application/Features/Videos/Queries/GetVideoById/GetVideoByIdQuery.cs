using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Videos.DTOs;

namespace Stock_Exchange.Application.Features.Videos.Queries.GetVideoById
{
    public class GetVideoByIdQuery : IRequest<Result<VideoDto>>
    {
        public Guid Id { get; set; }
        public bool? ApplyLanguageFilter { get; set; }

        public GetVideoByIdQuery()
        {
        }

        public GetVideoByIdQuery(Guid id, bool? applyLanguageFilter = null)
        {
            Id = id;
            ApplyLanguageFilter = applyLanguageFilter;
        }
    }
}

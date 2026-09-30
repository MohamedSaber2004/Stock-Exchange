using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Home.DTOs;

namespace Stock_Exchange.Application.Features.Home.Commands.UpdateHomeVideos
{
    public class UpdateHomeVideosCommand : IRequest<Result<List<HomeVideoDto>>>
    {
        public List<HomeVideoItemRequest> Items { get; set; } = new();

        public UpdateHomeVideosCommand() { }

        public UpdateHomeVideosCommand(List<HomeVideoItemRequest> items)
        {
            Items = items;
        }
    }
}

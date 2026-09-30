using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Home.DTOs;

namespace Stock_Exchange.Application.Features.Home.Commands.UpdateHomeNews
{
    public class UpdateHomeNewsCommand : IRequest<Result<List<HomeNewsDto>>>
    {
        public List<HomeNewsItemRequest> Items { get; set; } = new();

        public UpdateHomeNewsCommand() { }

        public UpdateHomeNewsCommand(List<HomeNewsItemRequest> items)
        {
            Items = items;
        }
    }
}

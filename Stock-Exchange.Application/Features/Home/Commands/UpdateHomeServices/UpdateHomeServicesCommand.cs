using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Home.DTOs;

namespace Stock_Exchange.Application.Features.Home.Commands.UpdateHomeServices
{
    public class UpdateHomeServicesCommand : IRequest<Result<List<HomeServiceDto>>>
    {
        public List<HomeServiceItemRequest> Items { get; set; } = new();

        public UpdateHomeServicesCommand() { }

        public UpdateHomeServicesCommand(List<HomeServiceItemRequest> items)
        {
            Items = items;
        }
    }
}

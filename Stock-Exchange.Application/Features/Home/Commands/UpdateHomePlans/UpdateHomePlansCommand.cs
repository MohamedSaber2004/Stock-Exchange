using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Home.DTOs;

namespace Stock_Exchange.Application.Features.Home.Commands.UpdateHomePlans
{
    public class UpdateHomePlansCommand : IRequest<Result<List<HomePlanDto>>>
    {
        public List<HomePlanItemRequest> Items { get; set; } = new();

        public UpdateHomePlansCommand() { }

        public UpdateHomePlansCommand(List<HomePlanItemRequest> items)
        {
            Items = items;
        }
    }
}

using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Home.DTOs;

namespace Stock_Exchange.Application.Features.Home.Commands.UpdateHomeExperts
{
    public class UpdateHomeExpertsCommand : IRequest<Result<List<HomeExpertDto>>>
    {
        public List<HomeExpertItemRequest> Items { get; set; } = new();

        public UpdateHomeExpertsCommand() { }

        public UpdateHomeExpertsCommand(List<HomeExpertItemRequest> items)
        {
            Items = items;
        }
    }
}

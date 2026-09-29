using MediatR;
using Stock_Exchange.Application.Common.Models;

namespace Stock_Exchange.Application.Features.HelpCenter.Commands.DeleteHelpCenter
{
    public class DeleteHelpCenterCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }

        public DeleteHelpCenterCommand()
        {
        }

        public DeleteHelpCenterCommand(Guid id)
        {
            Id = id;
        }
    }
}

using MediatR;
using Stock_Exchange.Application.Common.Models;

namespace Stock_Exchange.Application.Features.Videos.Commands.DeleteVideo
{
    public class DeleteVideoCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }

        public DeleteVideoCommand()
        {
        }

        public DeleteVideoCommand(Guid id)
        {
            Id = id;
        }
    }
}

using MediatR;
using Stock_Exchange.Application.Common.Models;

namespace Stock_Exchange.Application.Features.VideoCategories.Commands.DeleteVideoCategory
{
    public class DeleteVideoCategoryCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }
        public DeleteVideoCategoryCommand() { }
        public DeleteVideoCategoryCommand(Guid id) => Id = id;
    }
}

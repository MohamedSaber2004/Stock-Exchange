using MediatR;
using Stock_Exchange.Application.Common.Models;

namespace Stock_Exchange.Application.Features.HelpCenterCategories.Commands.DeleteHelpCenterCategory
{
    public class DeleteHelpCenterCategoryCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }

        public DeleteHelpCenterCategoryCommand()
        {
        }

        public DeleteHelpCenterCategoryCommand(Guid id)
        {
            Id = id;
        }
    }
}

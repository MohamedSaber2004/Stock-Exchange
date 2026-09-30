using MediatR;
using Stock_Exchange.Application.Common.Models;

namespace Stock_Exchange.Application.Features.TermsAndConditions.Commands.DeleteTermsAndConditions
{
    public class DeleteTermsAndConditionsCommand : IRequest<Result<bool>>
    {
        public Guid? Id { get; set; }

        public DeleteTermsAndConditionsCommand()
        {
        }

        public DeleteTermsAndConditionsCommand(Guid? id)
        {
            Id = id;
        }
    }
}

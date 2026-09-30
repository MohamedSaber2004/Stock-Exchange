using MediatR;
using Stock_Exchange.Application.Common.Models;

namespace Stock_Exchange.Application.Features.PrivacyPolicy.Commands.DeletePrivacy
{
    public class DeletePrivacyCommand : IRequest<Result<bool>>
    {
        public Guid? Id { get; set; }

        public DeletePrivacyCommand()
        {
        }

        public DeletePrivacyCommand(Guid? id)
        {
            Id = id;
        }
    }
}

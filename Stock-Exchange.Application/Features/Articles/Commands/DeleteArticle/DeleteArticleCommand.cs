using MediatR;
using Stock_Exchange.Application.Common.Models;

namespace Stock_Exchange.Application.Features.Articles.Commands.DeleteArticle
{
    public class DeleteArticleCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }

        public DeleteArticleCommand()
        {
        }

        public DeleteArticleCommand(Guid id)
        {
            Id = id;
        }
    }
}

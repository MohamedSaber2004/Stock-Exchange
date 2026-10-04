using MediatR;
using Stock_Exchange.Application.Common.Models;

namespace Stock_Exchange.Application.Features.ArticleCategories.Commands.DeleteArticleCategory
{
    public class DeleteArticleCategoryCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }
        public DeleteArticleCategoryCommand() { }
        public DeleteArticleCategoryCommand(Guid id) => Id = id;
    }
}

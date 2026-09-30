using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Home.DTOs;

namespace Stock_Exchange.Application.Features.Home.Commands.UpdateHomeArticles
{
    public class UpdateHomeArticlesCommand : IRequest<Result<List<HomeArticleDto>>>
    {
        public List<HomeArticleItemRequest> Items { get; set; } = new();

        public UpdateHomeArticlesCommand() { }

        public UpdateHomeArticlesCommand(List<HomeArticleItemRequest> items)
        {
            Items = items;
        }
    }
}

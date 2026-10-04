using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.ArticleCategories.DTOs;

namespace Stock_Exchange.Application.Features.ArticleCategories.Commands.AddArticleCategory
{
    public class AddArticleCategoryCommand : IRequest<Result<ArticleCategoryDto>>
    {
        public string CategoryArName { get; set; } = string.Empty;
        public string CategoryEnName { get; set; } = string.Empty;
    }
}

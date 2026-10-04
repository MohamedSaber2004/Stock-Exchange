using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.ArticleCategories.DTOs;

namespace Stock_Exchange.Application.Features.ArticleCategories.Commands.UpdateArticleCategory
{
    public class UpdateArticleCategoryCommand : IRequest<Result<ArticleCategoryDto>>
    {
        public Guid Id { get; set; }
        public string CategoryArName { get; set; } = string.Empty;
        public string CategoryEnName { get; set; } = string.Empty;
    }
}

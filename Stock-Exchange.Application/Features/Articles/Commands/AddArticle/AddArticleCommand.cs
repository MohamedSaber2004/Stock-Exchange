using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Articles.DTOs;

namespace Stock_Exchange.Application.Features.Articles.Commands.AddArticle
{
    public class AddArticleCommand : IRequest<Result<ArticleDto>>
    {
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string ExcerptEn { get; set; } = string.Empty;
        public string ExcerptAr { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public DateTime? PublishedAt { get; set; }
        public bool IsFeaturedOnHome { get; set; } = true;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

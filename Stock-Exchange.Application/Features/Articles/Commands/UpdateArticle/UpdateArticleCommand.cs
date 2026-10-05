using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Articles.DTOs;

namespace Stock_Exchange.Application.Features.Articles.Commands.UpdateArticle
{
    public class UpdateArticleCommand : IRequest<Result<ArticleDto>>
    {
        public Guid Id { get; set; }
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string ExcerptEn { get; set; } = string.Empty;
        public string ExcerptAr { get; set; } = string.Empty;
        public string? ContentEn { get; set; }
        public string? ContentAr { get; set; }
        public string? ImageUrl { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public int? ReadMinutes { get; set; }
        public DateTime? PublishedAt { get; set; }
        public bool IsFeaturedOnHome { get; set; } = true;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid? CategoryId { get; set; }
    }
}

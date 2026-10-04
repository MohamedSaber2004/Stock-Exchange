using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.News.DTOs;

namespace Stock_Exchange.Application.Features.News.Commands.AddNews
{
    public class AddNewsCommand : IRequest<Result<NewsDto>>
    {
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string SummaryEn { get; set; } = string.Empty;
        public string SummaryAr { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string CategoryEn { get; set; } = string.Empty;
        public string CategoryAr { get; set; } = string.Empty;
        public DateTime? PublishedAt { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsFeaturedOnHome { get; set; } = true;
        public bool IsActive { get; set; } = true;
    }
}

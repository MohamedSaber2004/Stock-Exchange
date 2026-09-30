using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Articles.DTOs;

namespace Stock_Exchange.Application.Features.Articles.Queries.GetArticleById
{
    public class GetArticleByIdQuery : IRequest<Result<ArticleDto>>
    {
        public Guid Id { get; set; }
        public bool? ApplyLanguageFilter { get; set; }

        public GetArticleByIdQuery()
        {
        }

        public GetArticleByIdQuery(Guid id, bool? applyLanguageFilter = null)
        {
            Id = id;
            ApplyLanguageFilter = applyLanguageFilter;
        }
    }
}

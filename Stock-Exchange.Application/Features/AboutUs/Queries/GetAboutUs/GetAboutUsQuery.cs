using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.AboutUs.DTOs;

namespace Stock_Exchange.Application.Features.AboutUs.Queries.GetAboutUs
{
    public class GetAboutUsQuery : IRequest<Result<AboutUsDto>>
    {
        public bool? ApplyLanguageFilter { get; set; }

        public GetAboutUsQuery()
        {
        }

        public GetAboutUsQuery(bool? applyLanguageFilter = null)
        {
            ApplyLanguageFilter = applyLanguageFilter;
        }
    }
}

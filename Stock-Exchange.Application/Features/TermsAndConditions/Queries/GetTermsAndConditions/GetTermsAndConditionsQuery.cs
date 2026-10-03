using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.TermsAndConditions.DTOs;

namespace Stock_Exchange.Application.Features.TermsAndConditions.Queries.GetTermsAndConditions
{
    public class GetTermsAndConditionsQuery : IRequest<Result<TermsAndConditionsDto>>
    {
        public bool? ApplyLanguageFilter { get; set; }

        public GetTermsAndConditionsQuery()
        {
        }

        public GetTermsAndConditionsQuery(bool? applyLanguageFilter = null)
        {
            ApplyLanguageFilter = applyLanguageFilter;
        }
    }
}

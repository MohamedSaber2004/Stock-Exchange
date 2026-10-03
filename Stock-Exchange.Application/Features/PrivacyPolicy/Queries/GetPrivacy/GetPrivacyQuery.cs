using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.PrivacyPolicy.DTOs;

namespace Stock_Exchange.Application.Features.PrivacyPolicy.Queries.GetPrivacy
{
    public class GetPrivacyQuery : IRequest<Result<PrivacyPolicyDto>>
    {
        public bool? ApplyLanguageFilter { get; set; }

        public GetPrivacyQuery()
        {
        }

        public GetPrivacyQuery(bool? applyLanguageFilter = null)
        {
            ApplyLanguageFilter = applyLanguageFilter;
        }
    }
}

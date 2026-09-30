using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.PrivacyPolicy.DTOs;

namespace Stock_Exchange.Application.Features.PrivacyPolicy.Commands.UpdatePrivacy
{
    public class UpdatePrivacyCommand : IRequest<Result<PrivacyPolicyDto>>
    {
        public string? TitleEn { get; set; }
        public string? TitleAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public List<PrivacyPolicySectionRequest>? Sections { get; set; }

        public UpdatePrivacyCommand()
        {
        }

        public UpdatePrivacyCommand(
            string? titleEn = null,
            string? titleAr = null,
            string? descriptionEn = null,
            string? descriptionAr = null,
            List<PrivacyPolicySectionRequest>? sections = null)
        {
            TitleEn = titleEn;
            TitleAr = titleAr;
            DescriptionEn = descriptionEn;
            DescriptionAr = descriptionAr;
            Sections = sections;
        }
    }
}

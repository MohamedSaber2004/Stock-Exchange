using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.TermsAndConditions.DTOs;

namespace Stock_Exchange.Application.Features.TermsAndConditions.Commands.UpdateTermsAndConditions
{
    public class UpdateTermsAndConditionsCommand : IRequest<Result<TermsAndConditionsDto>>
    {
        public string? TitleEn { get; set; }
        public string? TitleAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public List<TermsAndConditionsSectionRequest>? Sections { get; set; }

        public UpdateTermsAndConditionsCommand()
        {
        }

        public UpdateTermsAndConditionsCommand(
            string? titleEn = null,
            string? titleAr = null,
            string? descriptionEn = null,
            string? descriptionAr = null,
            List<TermsAndConditionsSectionRequest>? sections = null)
        {
            TitleEn = titleEn;
            TitleAr = titleAr;
            DescriptionEn = descriptionEn;
            DescriptionAr = descriptionAr;
            Sections = sections;
        }
    }
}

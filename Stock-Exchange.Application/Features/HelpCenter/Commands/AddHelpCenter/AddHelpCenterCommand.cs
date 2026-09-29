using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.HelpCenter.DTOs;

namespace Stock_Exchange.Application.Features.HelpCenter.Commands.AddHelpCenter
{
    public class AddHelpCenterCommand : IRequest<Result<HelpCenterDto>>
    {
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string ContentEn { get; set; } = string.Empty;
        public string ContentAr { get; set; } = string.Empty;
        public Guid? CategoryId { get; set; }

        public AddHelpCenterCommand()
        {
        }

        public AddHelpCenterCommand(
            string titleEn,
            string titleAr,
            string contentEn,
            string contentAr,
            Guid? categoryId)
        {
            TitleEn = titleEn;
            TitleAr = titleAr;
            ContentEn = contentEn;
            ContentAr = contentAr;
            CategoryId = categoryId;
        }
    }
}

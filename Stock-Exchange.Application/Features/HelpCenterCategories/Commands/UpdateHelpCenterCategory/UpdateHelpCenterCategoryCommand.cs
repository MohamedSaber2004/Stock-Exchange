using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.HelpCenterCategories.DTOs;

namespace Stock_Exchange.Application.Features.HelpCenterCategories.Commands.UpdateHelpCenterCategory
{
    public class UpdateHelpCenterCategoryCommand : IRequest<Result<HelpCenterCategoryDto>>
    {
        public Guid Id { get; set; }
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;

        public UpdateHelpCenterCategoryCommand()
        {
        }

        public UpdateHelpCenterCategoryCommand(Guid id, string titleEn, string titleAr)
        {
            Id = id;
            TitleEn = titleEn;
            TitleAr = titleAr;
        }
    }
}

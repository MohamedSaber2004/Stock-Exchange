using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.HelpCenterCategories.DTOs;

namespace Stock_Exchange.Application.Features.HelpCenterCategories.Commands.AddHelpCenterCategory
{
    public class AddHelpCenterCategoryCommand : IRequest<Result<HelpCenterCategoryDto>>
    {
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;

        public AddHelpCenterCategoryCommand()
        {
        }

        public AddHelpCenterCategoryCommand(string titleEn, string titleAr)
        {
            TitleEn = titleEn;
            TitleAr = titleAr;
        }
    }
}

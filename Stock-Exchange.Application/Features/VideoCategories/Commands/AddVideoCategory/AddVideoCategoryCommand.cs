using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.VideoCategories.DTOs;

namespace Stock_Exchange.Application.Features.VideoCategories.Commands.AddVideoCategory
{
    public class AddVideoCategoryCommand : IRequest<Result<VideoCategoryDto>>
    {
        public string CategoryArName { get; set; } = string.Empty;
        public string CategoryEnName { get; set; } = string.Empty;
    }
}

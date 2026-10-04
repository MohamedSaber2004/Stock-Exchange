using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.VideoCategories.DTOs;

namespace Stock_Exchange.Application.Features.VideoCategories.Commands.UpdateVideoCategory
{
    public class UpdateVideoCategoryCommand : IRequest<Result<VideoCategoryDto>>
    {
        public Guid Id { get; set; }
        public string CategoryArName { get; set; } = string.Empty;
        public string CategoryEnName { get; set; } = string.Empty;
    }
}

using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Services.DTOs;

namespace Stock_Exchange.Application.Features.Services.Commands.UpdateService
{
    public class UpdateServiceCommand : IRequest<Result<ServiceDto>>
    {
        public Guid Id { get; set; }
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;
        public string DescriptionAr { get; set; } = string.Empty;
        public string? IconName { get; set; }
        public string? ImageUrl { get; set; }
        public string? LinkRoute { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

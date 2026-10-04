using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Experts.DTOs;

namespace Stock_Exchange.Application.Features.Experts.Commands.UpdateExpert
{
    public class UpdateExpertCommand : IRequest<Result<ExpertDto>>
    {
        public Guid Id { get; set; }
        public string FullNameEn { get; set; } = string.Empty;
        public string FullNameAr { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsFeaturedOnHome { get; set; } = true;
        public bool IsActive { get; set; } = true;
    }
}

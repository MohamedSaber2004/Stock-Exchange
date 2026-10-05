using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Videos.DTOs;

namespace Stock_Exchange.Application.Features.Videos.Commands.AddVideo
{
    public class AddVideoCommand : IRequest<Result<VideoDto>>
    {
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? VideoUrl { get; set; }
        public int DurationSeconds { get; set; }
        public string InstructorName { get; set; } = string.Empty;
        public string CategoryEn { get; set; } = string.Empty;
        public string CategoryAr { get; set; } = string.Empty;
        public bool IsPreviewable { get; set; } = true;
        public bool IsFeaturedOnHome { get; set; } = true;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid? CategoryId { get; set; }
    }
}

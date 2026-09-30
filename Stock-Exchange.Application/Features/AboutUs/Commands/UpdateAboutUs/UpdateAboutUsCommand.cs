using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.AboutUs.DTOs;

namespace Stock_Exchange.Application.Features.AboutUs.Commands.UpdateAboutUs
{
    public class UpdateAboutUsCommand : IRequest<Result<AboutUsDto>>
    {
        public string? StoryEn { get; set; }
        public string? StoryAr { get; set; }
        public string? MissionEn { get; set; }
        public string? MissionAr { get; set; }
        public string? VisionEn { get; set; }
        public string? VisionAr { get; set; }
        public string? SupportEmail { get; set; }
        public List<AboutUsFeatureRequest>? Features { get; set; }

        public UpdateAboutUsCommand()
        {
        }

        public UpdateAboutUsCommand(
            string? storyEn = null,
            string? storyAr = null,
            string? missionEn = null,
            string? missionAr = null,
            string? visionEn = null,
            string? visionAr = null,
            string? supportEmail = null,
            List<AboutUsFeatureRequest>? features = null)
        {
            StoryEn = storyEn;
            StoryAr = storyAr;
            MissionEn = missionEn;
            MissionAr = missionAr;
            VisionEn = visionEn;
            VisionAr = visionAr;
            SupportEmail = supportEmail;
            Features = features;
        }
    }
}

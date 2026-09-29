using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.AboutUs.DTOs;

namespace Stock_Exchange.Application.Features.AboutUs.Commands.UpdateAboutUs
{
    public class UpdateAboutUsCommand : IRequest<Result<AboutUsDto>>
    {
        public string StoryEn { get; set; } = string.Empty;
        public string StoryAr { get; set; } = string.Empty;
        public string MissionEn { get; set; } = string.Empty;
        public string MissionAr { get; set; } = string.Empty;
        public string VisionEn { get; set; } = string.Empty;
        public string VisionAr { get; set; } = string.Empty;
        public List<AboutUsFeatureRequest> Features { get; set; } = new();

        public UpdateAboutUsCommand()
        {
        }

        public UpdateAboutUsCommand(
            string storyEn,
            string storyAr,
            string missionEn,
            string missionAr,
            string visionEn,
            string visionAr,
            List<AboutUsFeatureRequest> features)
        {
            StoryEn = storyEn;
            StoryAr = storyAr;
            MissionEn = missionEn;
            MissionAr = missionAr;
            VisionEn = visionEn;
            VisionAr = visionAr;
            Features = features;
        }
    }
}

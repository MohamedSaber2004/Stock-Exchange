using Stock_Exchange.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.AboutUs.Commands.UpdateAboutUs;
using Stock_Exchange.Application.Features.AboutUs.DTOs;
using Stock_Exchange.Application.Features.AboutUs.Queries.GetAboutUs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Enums;
using Stock_Exchange.Filters;
using Stock_Exchange.Routes.V1;
using System.Text;

namespace Stock_Exchange.Controllers.V1;

[ApiVersion("1.0")]
[Route(ApiRoutes.AboutUs.Base)]
public class AboutUsController : BaseController
{
    /// <summary>
    /// Retrieves the about us content, including the story, mission, vision and the core pillars.
    /// The response language is taken from the request Accept-Language header.
    /// </summary>
    /// <returns>The about us content in the requested language.</returns>
    /// <response code="200">About us content retrieved successfully.</response>
    /// <response code="404">About us content was not found.</response>
    [HttpGet]
    [Route(ApiRoutes.AboutUs.Get)]
    [RoleAuthorize]
    [ProducesResponseType(typeof(ApiResponse<AboutUsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AboutUsDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromQuery] bool? applyLanguageFilter = null)
    {
        var result = await Mediator.Send(new GetAboutUsQuery(applyLanguageFilter));

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Ok);
    }

    /// <summary>
    /// Returns a styled HTML page displaying the active About Us content.
    /// The display language is determined by the Accept-Language request header.
    /// </summary>
    /// <returns>An HTML page rendering the About Us content in the requested language.</returns>
    /// <response code="200">HTML page returned successfully.</response>
    /// <response code="404">About Us content was not found.</response>
    [HttpGet]
    [Route(ApiRoutes.AboutUs.View)]
    [AllowAnonymous]
    [Produces("text/html")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetView([FromQuery] string? lang = null)
    {
        var languageService = HttpContext.RequestServices.GetRequiredService<Stock_Exchange.Application.Common.Interfaces.ICurrentLanguageService>();
        var result = await Mediator.Send(new GetAboutUsQuery(false));

        if (!result.IsSuccess)
            return NotFound("About us content not found.");

        var data = result.Data!;

        var isAr = languageService.Language == Language.ar;
        var dir = isAr ? "rtl" : "ltr";
        var pageLang = isAr ? "ar" : "en";
        var pageTitle = isAr ? "من نحن" : "About Us";

        var sb = new StringBuilder();

        var story = isAr ? data.StoryAr : data.StoryEn;
        if (!string.IsNullOrWhiteSpace(story))
        {
            var storyLabel = isAr ? "قصتنا" : "Our Story";
            sb.Append($"<p class='section-label'>{System.Net.WebUtility.HtmlEncode(storyLabel)}</p>");
            sb.Append("<div class='section-card'>");
            sb.Append($"<p class='section-content'>{System.Net.WebUtility.HtmlEncode(story)}</p>");
            sb.Append("</div>");
        }

        var mission = isAr ? data.MissionAr : data.MissionEn;
        if (!string.IsNullOrWhiteSpace(mission))
        {
            var missionLabel = isAr ? "مهمتنا" : "Our Mission";
            sb.Append($"<p class='section-label'>{System.Net.WebUtility.HtmlEncode(missionLabel)}</p>");
            sb.Append("<div class='section-card'>");
            sb.Append($"<p class='section-content'>{System.Net.WebUtility.HtmlEncode(mission)}</p>");
            sb.Append("</div>");
        }

        var vision = isAr ? data.VisionAr : data.VisionEn;
        if (!string.IsNullOrWhiteSpace(vision))
        {
            var visionLabel = isAr ? "رؤيتنا" : "Our Vision";
            sb.Append($"<p class='section-label'>{System.Net.WebUtility.HtmlEncode(visionLabel)}</p>");
            sb.Append("<div class='section-card'>");
            sb.Append($"<p class='section-content'>{System.Net.WebUtility.HtmlEncode(vision)}</p>");
            sb.Append("</div>");
        }

        if (data.Features.Any())
        {
            var featuresLabel = isAr ? "ميزاتنا" : "Our Features";
            sb.Append($"<p class='section-label'>{System.Net.WebUtility.HtmlEncode(featuresLabel)}</p>");
            sb.Append("<div class='section-card'>");
            foreach (var feature in data.Features)
            {
                var featureTitle = isAr ? feature.TitleAr : feature.TitleEn;
                sb.Append("<div class='feature-row'>");
                sb.Append("<span class='feature-check'><svg width='12' height='12' viewBox='0 0 24 24' fill='none' stroke='#00b074' stroke-width='3' stroke-linecap='round' stroke-linejoin='round'><polyline points='20 6 9 17 4 12'/></svg></span>");
                sb.Append($"<span class='feature-text'>{System.Net.WebUtility.HtmlEncode(featureTitle)}</span>");
                sb.Append("</div>");
            }
            sb.Append("</div>");
        }

        if (!string.IsNullOrWhiteSpace(data.SupportEmail))
        {
            var connectLabel = isAr ? "تواصل معنا" : "Connect With Us";
            var emailLabel = isAr ? "بريد الدعم" : "Support Email";
            sb.Append($"<p class='section-label'>{System.Net.WebUtility.HtmlEncode(connectLabel)}</p>");
            sb.Append("<div class='section-card'>");
            sb.Append("<div class='connect-row'>");
            sb.Append("<div class='connect-row-left'>");
            sb.Append("<span class='connect-icon'><svg width='18' height='18' viewBox='0 0 24 24' fill='none' stroke='#00b074' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><circle cx='12' cy='12' r='10'/><line x1='2' y1='12' x2='22' y2='12'/><path d='M12 2a15.3 15.3 0 0 1 4 10 15.3 15.3 0 0 1-4 10 15.3 15.3 0 0 1-4-10 15.3 15.3 0 0 1 4-10z'/></svg></span>");
            sb.Append("<div>");
            sb.Append($"<div class='connect-label'>{System.Net.WebUtility.HtmlEncode(emailLabel)}</div>");
            sb.Append($"<div class='connect-sub'>{System.Net.WebUtility.HtmlEncode(data.SupportEmail)}</div>");
            sb.Append("</div></div>");
            sb.Append("<span class='connect-chevron'><svg width='16' height='16' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'><polyline points='9 18 15 12 9 6'/></svg></span>");
            sb.Append("</div>");
            sb.Append("</div>");
        }

        var html = StaticPageRenderer.BuildDocument(pageTitle, pageLang, dir, sb.ToString());
        return Content(html, "text/html", Encoding.UTF8);
    }
    

    /// <summary>
    /// Creates or updates the about us content (supports partial update) along with its core pillars and support email.
    /// </summary>
    /// <param name="command">The about us fields, support email, and core pillars to update.</param>
    /// <returns>The saved about us content.</returns>
    /// <response code="200">About us content saved successfully.</response>
    /// <response code="400">One or more fields exceed the allowed length or are invalid.</response>
    /// <response code="401">The caller is not authenticated.</response>
    [HttpPatch]
    [Route(ApiRoutes.AboutUs.Update)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<AboutUsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AboutUsDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromBody] UpdateAboutUsCommand command)
    {
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Updated);
    }
}

using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.TermsAndConditions.Commands.DeleteTermsAndConditions;
using Stock_Exchange.Application.Features.TermsAndConditions.Commands.UpdateTermsAndConditions;
using Stock_Exchange.Application.Features.TermsAndConditions.DTOs;
using Stock_Exchange.Application.Features.TermsAndConditions.Queries.GetTermsAndConditions;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Enums;
using Stock_Exchange.Filters;
using Stock_Exchange.Routes.V1;
using System.Text;

namespace Stock_Exchange.Controllers.V1;

[ApiVersion("1.0")]
[Route(ApiRoutes.TermsAndConditions.Base)]
public class TermsAndConditionsController : BaseController
{
    /// <summary>
    /// Retrieves the terms and conditions content and its sections.
    /// The response language is taken from the request Accept-Language header.
    /// </summary>
    /// <returns>The terms and conditions in the requested language.</returns>
    /// <response code="200">Terms and conditions retrieved successfully.</response>
    /// <response code="404">Terms and conditions was not found.</response>
    [HttpGet]
    [Route(ApiRoutes.TermsAndConditions.Get)]
    [RoleAuthorize]
    [ProducesResponseType(typeof(ApiResponse<TermsAndConditionsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<TermsAndConditionsDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromQuery] bool? applyLanguageFilter = null)
    {
        var result = await Mediator.Send(new GetTermsAndConditionsQuery(applyLanguageFilter));

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Ok);
    }

    /// <summary>
    /// Returns a styled HTML page displaying the active terms and conditions.
    /// The display language is determined by the Accept-Language request header.
    /// </summary>
    /// <returns>An HTML page rendering the terms and conditions content in the requested language.</returns>
    /// <response code="200">HTML page returned successfully.</response>
    /// <response code="404">Terms and conditions was not found.</response>
    [HttpGet]
    [Route(ApiRoutes.TermsAndConditions.View)]
    [AllowAnonymous]
    [Produces("text/html")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetView([FromQuery] string? lang = null)
    {
        var languageService = HttpContext.RequestServices.GetRequiredService<Stock_Exchange.Application.Common.Interfaces.ICurrentLanguageService>();
        var result = await Mediator.Send(new GetTermsAndConditionsQuery(false));

        if (!result.IsSuccess)
            return NotFound("Terms and conditions not found.");

        var data = result.Data!;

        var isAr = languageService.Language == Language.ar;
        var dir = isAr ? "rtl" : "ltr";
        var pageLang = isAr ? "ar" : "en";
        var pageTitle = isAr ? "الشروط والأحكام" : "Terms & Conditions";
        var emptyMsg = isAr ? "لا يوجد محتوى متاح." : "No content available.";

        var sb = new StringBuilder();
        sb.Append($"<!DOCTYPE html><html lang='{pageLang}' dir='{dir}'><head>");
        sb.Append("<meta charset='UTF-8'/><meta name='viewport' content='width=device-width, initial-scale=1.0'/>");
        sb.Append($"<title>{System.Net.WebUtility.HtmlEncode(pageTitle)}</title>");
        sb.Append("<link rel='icon' href='/favicon.ico' type='image/x-icon'/>");
        sb.Append("<link rel='stylesheet' href='/pages/pages.css'/></head><body>");
        sb.Append("<header class='page-header'>");
        sb.Append($"<h1 class='page-title'>{System.Net.WebUtility.HtmlEncode(pageTitle)}</h1>");
        sb.Append("</header>");
        sb.Append("<main class='page-container'>");

        var description = isAr ? data.DescriptionAr : data.DescriptionEn;
        if (!string.IsNullOrWhiteSpace(description))
            sb.Append($"<div class='intro-banner'><p>{System.Net.WebUtility.HtmlEncode(description)}</p></div>");

        int idx = 1;
        foreach (var section in data.Sections)
        {
            var title = isAr ? section.TitleAr : section.TitleEn;
            var content = isAr ? section.ContentAr : section.ContentEn;
            sb.Append("<div class='section-card'>");
            sb.Append($"<div class='section-title'><span class='section-number'>{idx}</span>");
            if (!string.IsNullOrWhiteSpace(title))
                sb.Append($"<h2>{System.Net.WebUtility.HtmlEncode(title)}</h2>");
            sb.Append("</div>");
            if (!string.IsNullOrWhiteSpace(content))
                sb.Append($"<p class='section-content'>{System.Net.WebUtility.HtmlEncode(content)}</p>");
            sb.Append("</div>");
            idx++;
        }

        if (!data.Sections.Any())
            sb.Append($"<div class='empty-state'><p>{emptyMsg}</p></div>");

        sb.Append("</main></body></html>");

        return Content(sb.ToString(), "text/html", Encoding.UTF8);
    }

    /// <summary>
    /// Creates or updates the terms and conditions content and its sections.
    /// </summary>
    /// <param name="command">The terms and conditions titles and sections to update.</param>
    /// <returns>The saved terms and conditions content.</returns>
    /// <response code="200">Terms and conditions saved successfully.</response>
    /// <response code="400">One or more fields exceed the allowed length or are invalid.</response>
    /// <response code="401">The caller is not authenticated.</response>
    /// <response code="403">The caller is not authorized.</response>
    [HttpPatch]
    [Route(ApiRoutes.TermsAndConditions.Update)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<TermsAndConditionsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<TermsAndConditionsDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromBody] UpdateTermsAndConditionsCommand command)
    {
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Updated);
    }

    /// <summary>
    /// Soft deletes the active terms and conditions.
    /// </summary>
    /// <returns>Whether the terms and conditions was deleted.</returns>
    /// <response code="200">Terms and conditions deleted successfully.</response>
    /// <response code="404">Terms and conditions was not found.</response>
    [HttpDelete]
    [Route(ApiRoutes.TermsAndConditions.Delete)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete()
    {
        var result = await Mediator.Send(new DeleteTermsAndConditionsCommand());

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Deleted);
    }

    /// <summary>
    /// Soft deletes a specific terms and conditions section or the terms and conditions by id.
    /// </summary>
    /// <param name="id">The id of the section or policy to delete.</param>
    /// <returns>Whether the entity was deleted.</returns>
    /// <response code="200">Entry deleted successfully.</response>
    /// <response code="404">Entry was not found.</response>
    [HttpDelete]
    [Route(ApiRoutes.TermsAndConditions.DeleteById)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteById(Guid id)
    {
        var result = await Mediator.Send(new DeleteTermsAndConditionsCommand(id));

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Deleted);
    }
}

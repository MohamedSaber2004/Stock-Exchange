using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.AboutUs.Commands.UpdateAboutUs;
using Stock_Exchange.Application.Features.AboutUs.DTOs;
using Stock_Exchange.Application.Features.AboutUs.Queries.GetAboutUs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Enums;
using Stock_Exchange.Filters;
using Stock_Exchange.Routes.V1;

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
    public async Task<IActionResult> Get()
    {
        var result = await Mediator.Send(new GetAboutUsQuery());

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Ok);
    }

    /// <summary>
    /// Creates or updates the about us content (supports partial update) along with its core pillars and support email.
    /// </summary>
    /// <param name="command">The about us fields, support email, and core pillars to update.</param>
    /// <returns>The saved about us content.</returns>
    /// <response code="200">About us content saved successfully.</response>
    /// <response code="400">One or more fields exceed the allowed length or are invalid.</response>
    /// <response code="401">The caller is not authenticated.</response>
    [HttpPut]
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

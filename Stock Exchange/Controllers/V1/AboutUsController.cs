using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AboutUsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AboutUsDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Get()
    {
        var result = await Mediator.Send(new GetAboutUsQuery());

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Ok);
    }

    /// <summary>
    /// Creates or replaces the about us content along with its core pillars.
    /// </summary>
    /// <param name="command">The story, mission, vision and the list of core pillars to save.</param>
    /// <returns>The saved about us content.</returns>
    /// <response code="200">About us content saved successfully.</response>
    /// <response code="400">One or more fields are missing or exceed the allowed length.</response>
    /// <response code="401">The caller is not authenticated.</response>
    [HttpPut]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<AboutUsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AboutUsDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Update([FromBody] UpdateAboutUsCommand command)
    {
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Updated);
    }
}

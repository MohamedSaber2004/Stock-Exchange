using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Countries.Commands.AddCountry;
using Stock_Exchange.Application.Features.Countries.Commands.DeleteCountry;
using Stock_Exchange.Application.Features.Countries.Commands.UpdateCountry;
using Stock_Exchange.Application.Features.Countries.DTOs;
using Stock_Exchange.Application.Features.Countries.Queries.GetAllCountries;
using Stock_Exchange.Application.Features.Countries.Queries.GetAllCountriesPaginated;
using Stock_Exchange.Application.Features.Countries.Queries.GetCountryById;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Enums;
using Stock_Exchange.Filters;
using Stock_Exchange.Routes.V1;

namespace Stock_Exchange.Controllers.V1;

[ApiVersion("1.0")]
[Route(ApiRoutes.Countries.Base)]
public class CountriesController : BaseController
{
    /// <summary>
    /// Retrieves all active countries for public dropdown selectors.
    /// </summary>
    [HttpGet]
    [Route(ApiRoutes.Countries.GetAll)]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<List<CountryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await Mediator.Send(new GetAllCountriesQuery());
        return OkResult(result, LocalizationKeys.ActionResults.Ok);
    }

    /// <summary>
    /// Retrieves paginated list of countries for the admin dashboard.
    /// </summary>
    [HttpGet]
    [Route(ApiRoutes.Countries.GetAllPaginated)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<PagginatedResult<CountryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPaginated([FromQuery] GetAllCountriesPaginatedQuery query)
    {
        var result = await Mediator.Send(query);
        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Ok);
    }

    /// <summary>
    /// Retrieves country details by ID.
    /// </summary>
    [HttpGet]
    [Route(ApiRoutes.Countries.GetById)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<CountryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await Mediator.Send(new GetCountryByIdQuery(id));
        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Ok);
    }

    /// <summary>
    /// Adds a new country to the system.
    /// </summary>
    [HttpPost]
    [Route(ApiRoutes.Countries.Create)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<CountryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] AddCountryCommand command)
    {
        var result = await Mediator.Send(command);
        if (!result.IsSuccess)
            return FromResult(result);

        return CreatedResult(result.Data, LocalizationKeys.ActionResults.Created);
    }

    /// <summary>
    /// Updates an existing country.
    /// </summary>
    [HttpPut]
    [Route(ApiRoutes.Countries.Update)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<CountryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCountryCommand command)
    {
        var targetCommand = command with { Id = id };
        var result = await Mediator.Send(targetCommand);
        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Updated);
    }

    /// <summary>
    /// Deletes a country from the system (only if no users are linked to it).
    /// </summary>
    [HttpDelete]
    [Route(ApiRoutes.Countries.Delete)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await Mediator.Send(new DeleteCountryCommand(id));
        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Deleted);
    }
}

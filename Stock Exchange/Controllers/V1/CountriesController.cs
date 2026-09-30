using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Countries.DTOs;
using Stock_Exchange.Application.Features.Countries.Queries.GetAllCountries;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Routes.V1;

namespace Stock_Exchange.Controllers.V1;

[ApiVersion("1.0")]
[Route(ApiRoutes.Countries.Base)]
public class CountriesController : BaseController
{
    /// <summary>
    /// Retrieves all active countries with their names and codes.
    /// </summary>
    /// <returns>A list of active countries.</returns>
    /// <response code="200">Countries retrieved successfully.</response>
    [HttpGet]
    [Route(ApiRoutes.Countries.GetAll)]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<List<CountryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await Mediator.Send(new GetAllCountriesQuery());
        return OkResult(result, LocalizationKeys.ActionResults.Ok);
    }
}

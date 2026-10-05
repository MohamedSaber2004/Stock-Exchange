using Microsoft.AspNetCore.Authorization;
﻿using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Services.Commands.AddService;
using Stock_Exchange.Application.Features.Services.Commands.DeleteService;
using Stock_Exchange.Application.Features.Services.Commands.UpdateService;
using Stock_Exchange.Application.Features.Services.DTOs;
using Stock_Exchange.Application.Features.Services.Queries.GetAllServices;
using Stock_Exchange.Application.Features.Services.Queries.GetServiceById;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Enums;
using Stock_Exchange.Filters;
using Stock_Exchange.Routes.V1;

namespace Stock_Exchange.Controllers.V1;

[ApiVersion("1.0")]
[Route(ApiRoutes.Services.Base)]
public class ServicesController : BaseController
{
    [HttpGet]
    [Route(ApiRoutes.Services.GetAll)]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PagginatedResult<ServiceDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllServicesQuery query)
    {
        var result = await Mediator.Send(query);
        if (!result.IsSuccess) return FromResult(result);
        return OkResult(result.Data, LocalizationKeys.ActionResults.Ok);
    }

    [HttpGet]
    [Route(ApiRoutes.Services.GetById)]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<ServiceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id, [FromQuery] bool? applyLanguageFilter = null)
    {
        var result = await Mediator.Send(new GetServiceByIdQuery(id, applyLanguageFilter));
        if (!result.IsSuccess) return FromResult(result);
        return OkResult(result.Data, LocalizationKeys.ActionResults.Ok);
    }

    [HttpPost]
    [Route(ApiRoutes.Services.Add)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<ServiceDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Add([FromBody] AddServiceCommand command)
    {
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return FromResult(result);
        return CreatedResult(result.Data, LocalizationKeys.ActionResults.Created);
    }

    [HttpPut]
    [Route(ApiRoutes.Services.Update)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<ServiceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateServiceCommand command)
    {
        command.Id = id;
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return FromResult(result);
        return OkResult(result.Data, LocalizationKeys.ActionResults.Updated);
    }

    [HttpDelete]
    [Route(ApiRoutes.Services.Delete)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await Mediator.Send(new DeleteServiceCommand(id));
        if (!result.IsSuccess) return FromResult(result);
        return OkResult(result.Data, LocalizationKeys.ActionResults.Deleted);
    }
}

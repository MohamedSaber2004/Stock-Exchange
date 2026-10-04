using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Experts.Commands.AddExpert;
using Stock_Exchange.Application.Features.Experts.Commands.DeleteExpert;
using Stock_Exchange.Application.Features.Experts.Commands.UpdateExpert;
using Stock_Exchange.Application.Features.Experts.DTOs;
using Stock_Exchange.Application.Features.Experts.Queries.GetAllExperts;
using Stock_Exchange.Application.Features.Experts.Queries.GetExpertById;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Enums;
using Stock_Exchange.Filters;
using Stock_Exchange.Routes.V1;

namespace Stock_Exchange.Controllers.V1;

[ApiVersion("1.0")]
[Route(ApiRoutes.Experts.Base)]
public class ExpertsController : BaseController
{
    [HttpGet]
    [Route(ApiRoutes.Experts.GetAll)]
    [RoleAuthorize]
    [ProducesResponseType(typeof(ApiResponse<PagginatedResult<ExpertDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllExpertsQuery query)
    {
        var result = await Mediator.Send(query);
        if (!result.IsSuccess) return FromResult(result);
        return OkResult(result.Data, LocalizationKeys.ActionResults.Ok);
    }

    [HttpGet]
    [Route(ApiRoutes.Experts.GetById)]
    [RoleAuthorize]
    [ProducesResponseType(typeof(ApiResponse<ExpertDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id, [FromQuery] bool? applyLanguageFilter = null)
    {
        var result = await Mediator.Send(new GetExpertByIdQuery(id, applyLanguageFilter));
        if (!result.IsSuccess) return FromResult(result);
        return OkResult(result.Data, LocalizationKeys.ActionResults.Ok);
    }

    [HttpPost]
    [Route(ApiRoutes.Experts.Add)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<ExpertDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Add([FromBody] AddExpertCommand command)
    {
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return FromResult(result);
        return CreatedResult(result.Data, LocalizationKeys.ActionResults.Created);
    }

    [HttpPut]
    [Route(ApiRoutes.Experts.Update)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<ExpertDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateExpertCommand command)
    {
        command.Id = id;
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return FromResult(result);
        return OkResult(result.Data, LocalizationKeys.ActionResults.Updated);
    }

    [HttpDelete]
    [Route(ApiRoutes.Experts.Delete)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await Mediator.Send(new DeleteExpertCommand(id));
        if (!result.IsSuccess) return FromResult(result);
        return OkResult(result.Data, LocalizationKeys.ActionResults.Deleted);
    }
}

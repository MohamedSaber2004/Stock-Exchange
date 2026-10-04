using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.News.Commands.AddNews;
using Stock_Exchange.Application.Features.News.Commands.DeleteNews;
using Stock_Exchange.Application.Features.News.Commands.UpdateNews;
using Stock_Exchange.Application.Features.News.DTOs;
using Stock_Exchange.Application.Features.News.Queries.GetAllNews;
using Stock_Exchange.Application.Features.News.Queries.GetNewsById;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Enums;
using Stock_Exchange.Filters;
using Stock_Exchange.Routes.V1;

namespace Stock_Exchange.Controllers.V1;

[ApiVersion("1.0")]
[Route(ApiRoutes.News.Base)]
public class NewsController : BaseController
{
    [HttpGet]
    [Route(ApiRoutes.News.GetAll)]
    [RoleAuthorize]
    [ProducesResponseType(typeof(ApiResponse<PagginatedResult<NewsDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllNewsQuery query)
    {
        var result = await Mediator.Send(query);
        if (!result.IsSuccess) return FromResult(result);
        return OkResult(result.Data, LocalizationKeys.ActionResults.Ok);
    }

    [HttpGet]
    [Route(ApiRoutes.News.GetById)]
    [RoleAuthorize]
    [ProducesResponseType(typeof(ApiResponse<NewsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id, [FromQuery] bool? applyLanguageFilter = null)
    {
        var result = await Mediator.Send(new GetNewsByIdQuery(id, applyLanguageFilter));
        if (!result.IsSuccess) return FromResult(result);
        return OkResult(result.Data, LocalizationKeys.ActionResults.Ok);
    }

    [HttpPost]
    [Route(ApiRoutes.News.Add)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<NewsDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Add([FromBody] AddNewsCommand command)
    {
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return FromResult(result);
        return CreatedResult(result.Data, LocalizationKeys.ActionResults.Created);
    }

    [HttpPut]
    [Route(ApiRoutes.News.Update)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<NewsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateNewsCommand command)
    {
        command.Id = id;
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return FromResult(result);
        return OkResult(result.Data, LocalizationKeys.ActionResults.Updated);
    }

    [HttpDelete]
    [Route(ApiRoutes.News.Delete)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await Mediator.Send(new DeleteNewsCommand(id));
        if (!result.IsSuccess) return FromResult(result);
        return OkResult(result.Data, LocalizationKeys.ActionResults.Deleted);
    }
}

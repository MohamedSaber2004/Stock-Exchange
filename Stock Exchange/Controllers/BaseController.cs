using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Routes;

namespace Stock_Exchange.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    private IMediator? _mediator;
    private IStringLocalizer<Messages>? _localizer;

    protected IMediator Mediator =>
        _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();

    protected IStringLocalizer<Messages> Localizer =>
        _localizer ??= HttpContext.RequestServices.GetRequiredService<IStringLocalizer<Messages>>();

    protected string Localize(string localizationKey, params object[] args) =>
        Localizer[localizationKey, args].Value;

    protected ActionResult FromResult<T>(Result<T>? result)
    {
        if (result is null)
            return InternalError<T>(LocalizationKeys.ExceptionMessages.UnknownException);

        return StatusCode(result.StatusCode, result);
    }

    protected ActionResult OkResult<T>(T? data, string localizationKey, params object[] args) =>
        StatusCode(
            StatusCodes.Status200OK,
            Result<T>.Success(data, Localize(localizationKey, args)));

    protected ActionResult CreatedResult<T>(T? data, string localizationKey, params object[] args) =>
        StatusCode(
            StatusCodes.Status201Created,
            Result<T>.Success(data, Localize(localizationKey, args), StatusCodes.Status201Created));

    protected ActionResult AcceptedResult<T>(T? data, string localizationKey, params object[] args) =>
        StatusCode(
            StatusCodes.Status202Accepted,
            Result<T>.Success(data, Localize(localizationKey, args), StatusCodes.Status202Accepted));

    protected ActionResult NoContentResult() => NoContent();

    protected ActionResult NotFoundResult<T>(
        string localizationKey,
        List<string>? errors = null,
        params object[] args) =>
        StatusCode(
            StatusCodes.Status404NotFound,
            Result<T>.Failure(Localize(localizationKey, args), StatusCodes.Status404NotFound, errors));

    private ActionResult InternalError<T>(string localizationKey, params object[] args) =>
        StatusCode(
            StatusCodes.Status500InternalServerError,
            Result<T>.Failure(
                Localize(localizationKey, args),
                StatusCodes.Status500InternalServerError));
}

using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.ArticleCategories.DTOs;
using Stock_Exchange.Application.Features.ArticleCategories.Queries.GetAllArticleCategories;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Filters;
using Stock_Exchange.Routes.V1;

namespace Stock_Exchange.Controllers.V1;

[ApiVersion("1.0")]
[Route(ApiRoutes.ArticleCategories.Base)]
[Route("api/v{version:apiVersion}/articles/categories")]
public class ArticleCategoriesController : BaseController
{
    /// <summary>
    /// Retrieves all active article categories, optionally filtered by search term.
    /// </summary>
    /// <param name="query">Optional search and language filter parameters.</param>
    /// <returns>List of article categories.</returns>
    /// <response code="200">Article categories retrieved successfully.</response>
    [HttpGet]
    [Route(ApiRoutes.ArticleCategories.GetAll)]
    [RoleAuthorize]
    [ProducesResponseType(typeof(ApiResponse<List<ArticleCategoryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllArticleCategoriesQuery query)
    {
        var result = await Mediator.Send(query);

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Ok);
    }
}

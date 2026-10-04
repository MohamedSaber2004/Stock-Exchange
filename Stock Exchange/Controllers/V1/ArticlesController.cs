using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Articles.Commands.AddArticle;
using Stock_Exchange.Application.Features.Articles.Commands.DeleteArticle;
using Stock_Exchange.Application.Features.Articles.Commands.UpdateArticle;
using Stock_Exchange.Application.Features.Articles.DTOs;
using Stock_Exchange.Application.Features.Articles.Queries.GetAllArticles;
using Stock_Exchange.Application.Features.Articles.Queries.GetArticleById;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Enums;
using Stock_Exchange.Filters;
using Stock_Exchange.Routes.V1;

namespace Stock_Exchange.Controllers.V1;

[ApiVersion("1.0")]
[Route(ApiRoutes.Articles.Base)]
public class ArticlesController : BaseController
{
    /// <summary>
    /// Retrieves paginated articles (default page size: 10), with optional search and active status filter.
    /// General authorization: Accessible to any authenticated user.
    /// </summary>
    /// <param name="query">Pagination parameters, optional search term, and filters.</param>
    /// <returns>A paginated list of articles.</returns>
    /// <response code="200">Articles retrieved successfully.</response>
    [HttpGet]
    [Route(ApiRoutes.Articles.GetAll)]
    [RoleAuthorize]
    [ProducesResponseType(typeof(ApiResponse<PagginatedResult<ArticleDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllArticlesQuery query)
    {
        var result = await Mediator.Send(query);

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Ok);
    }

    /// <summary>
    /// Retrieves a single article by its id.
    /// General authorization: Accessible to any authenticated user.
    /// </summary>
    /// <param name="id">The article id.</param>
    /// <param name="applyLanguageFilter">Optional flag to apply language filter based on Accept-Language header (default true).</param>
    /// <returns>The article data.</returns>
    /// <response code="200">Article retrieved successfully.</response>
    /// <response code="404">Article was not found.</response>
    [HttpGet]
    [Route(ApiRoutes.Articles.GetById)]
    [RoleAuthorize]
    [ProducesResponseType(typeof(ApiResponse<ArticleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ArticleDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, [FromQuery] bool? applyLanguageFilter = null)
    {
        var result = await Mediator.Send(new GetArticleByIdQuery(id, applyLanguageFilter));

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Ok);
    }

    /// <summary>
    /// Creates a new article.
    /// Admin authorization only.
    /// </summary>
    /// <param name="command">The article data to create.</param>
    /// <returns>The created article.</returns>
    /// <response code="201">Article created successfully.</response>
    /// <response code="400">One or more fields are invalid or exceed allowed lengths.</response>
    /// <response code="401">User is unauthorized.</response>
    /// <response code="403">User is forbidden (admin only).</response>
    [HttpPost]
    [Route(ApiRoutes.Articles.Add)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<ArticleDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<ArticleDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add([FromBody] AddArticleCommand command)
    {
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return FromResult(result);

        return CreatedResult(result.Data, LocalizationKeys.ActionResults.Created);
    }

    /// <summary>
    /// Updates an existing article by route id.
    /// Admin authorization only.
    /// </summary>
    /// <param name="id">The article id from route.</param>
    /// <param name="command">The updated properties.</param>
    /// <returns>The updated article.</returns>
    /// <response code="200">Article updated successfully.</response>
    /// <response code="400">One or more fields are invalid or exceed allowed lengths.</response>
    /// <response code="401">User is unauthorized.</response>
    /// <response code="403">User is forbidden (admin only).</response>
    /// <response code="404">Article was not found.</response>
    [HttpPut]
    [Route(ApiRoutes.Articles.Update)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<ArticleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ArticleDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<ArticleDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateArticleCommand command)
    {
        command.Id = id;
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Updated);
    }

    /// <summary>
    /// Hard deletes an article permanently from the database.
    /// Admin authorization only.
    /// </summary>
    /// <param name="id">The article id to permanently delete.</param>
    /// <returns>True if successfully hard deleted.</returns>
    /// <response code="200">Article permanently deleted successfully.</response>
    /// <response code="401">User is unauthorized.</response>
    /// <response code="403">User is forbidden (admin only).</response>
    /// <response code="404">Article was not found.</response>
    [HttpDelete]
    [Route(ApiRoutes.Articles.Delete)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await Mediator.Send(new DeleteArticleCommand(id));

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Deleted);
    }
}

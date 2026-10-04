using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.ArticleCategories.Commands.AddArticleCategory;
using Stock_Exchange.Application.Features.ArticleCategories.Commands.DeleteArticleCategory;
using Stock_Exchange.Application.Features.ArticleCategories.Commands.UpdateArticleCategory;
using Stock_Exchange.Application.Features.ArticleCategories.DTOs;
using Stock_Exchange.Application.Features.ArticleCategories.Queries.GetAllArticleCategories;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Enums;
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

    /// <summary>
    /// Creates a new article category.
    /// </summary>
    /// <param name="command">The category payload with English and Arabic names.</param>
    /// <returns>Created category.</returns>
    [HttpPost]
    [Route(ApiRoutes.ArticleCategories.Add)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<ArticleCategoryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<ArticleCategoryDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add([FromBody] AddArticleCategoryCommand command)
    {
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return FromResult(result);

        return CreatedResult(result.Data, LocalizationKeys.ActionResults.Created);
    }

    /// <summary>
    /// Updates an existing article category.
    /// </summary>
    /// <param name="command">The category update payload.</param>
    /// <param name="id">Optional category id from route.</param>
    /// <returns>Updated category.</returns>
    [HttpPut]
    [Route(ApiRoutes.ArticleCategories.Update)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<ArticleCategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ArticleCategoryDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<ArticleCategoryDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] UpdateArticleCategoryCommand command, [FromRoute] Guid? id = null)
    {
        if (id.HasValue && id.Value != Guid.Empty)
            command.Id = id.Value;

        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Updated);
    }

    /// <summary>
    /// Soft deletes an article category.
    /// </summary>
    /// <param name="id">Category ID.</param>
    /// <returns>True if deleted.</returns>
    [HttpDelete]
    [Route(ApiRoutes.ArticleCategories.Delete)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await Mediator.Send(new DeleteArticleCategoryCommand(id));

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Deleted);
    }
}

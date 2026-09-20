using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Attachments.Commands.DownloadFile;
using Stock_Exchange.Application.Features.Attachments.Commands.UpdateFile;
using Stock_Exchange.Application.Features.Attachments.Commands.UploadFile;
using Stock_Exchange.Application.Features.Attachments.Commands.UploadMultipleFiles;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Enums;
using Stock_Exchange.Filters;
using Stock_Exchange.Routes.V1;

namespace Stock_Exchange.Controllers.V1;

[ApiVersion("1.0")]
[Route(ApiRoutes.Attachments.Base)]
[RoleAuthorize]
public class AttachmentsController : BaseController
{
    /// <summary>
    /// Uploads a single attachment file.
    /// </summary>
    /// <param name="command">The upload request (file, media type and place) sent as multipart/form-data.</param>
    /// <returns>The stored file name.</returns>
    /// <response code="201">File uploaded successfully.</response>
    /// <response code="400">The file is missing or its format is not valid for the given media type.</response>
    [HttpPost]
    [Route(ApiRoutes.Attachments.Upload)]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Upload([FromForm] UploadFileCommand command)
    {
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return FromResult(result);

        return CreatedResult(result.Data, LocalizationKeys.ActionResults.Created);
    }

    /// <summary>
    /// Uploads multiple attachment files of the same media type.
    /// </summary>
    /// <param name="command">The upload request (files, media type and place) sent as multipart/form-data.</param>
    /// <returns>The comma-separated stored file names.</returns>
    /// <response code="201">Files uploaded successfully.</response>
    /// <response code="400">No files were provided or none of them is valid for the given media type.</response>
    [HttpPost]
    [Route(ApiRoutes.Attachments.UploadMultiple)]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UploadMultiple([FromForm] UploadMultipleFilesCommand command)
    {
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return FromResult(result);

        return CreatedResult(result.Data, LocalizationKeys.ActionResults.Created);
    }

    /// <summary>
    /// Downloads a previously uploaded attachment file.
    /// </summary>
    /// <param name="command">The download request (stored file name with place prefix, file place and media type).</param>
    /// <returns>The file content.</returns>
    /// <response code="200">File content.</response>
    /// <response code="404">The requested file was not found.</response>
    [HttpGet]
    [Route(ApiRoutes.Attachments.Download)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Download([FromQuery] DownloadFileCommand command)
    {
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return FromResult(result);

        var contentType = GetContentType(result.Data!);

        return PhysicalFile(result.Data!, contentType, Path.GetFileName(result.Data!));
    }

    /// <summary>
    /// Replaces an existing attachment file with a newly uploaded one.
    /// </summary>
    /// <param name="command">The update request (old file name, new file, media type and place) sent as multipart/form-data.</param>
    /// <returns>The new stored file name.</returns>
    /// <response code="200">File replaced successfully.</response>
    /// <response code="400">The old file name is missing, or the new file is missing or invalid.</response>
    [HttpPut]
    [Route(ApiRoutes.Attachments.Update)]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Update([FromForm] UpdateFileCommand command)
    {
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Updated);
    }

    private static string GetContentType(string filePath)
    {
        var provider = new FileExtensionContentTypeProvider();

        return provider.TryGetContentType(filePath, out var contentType)
            ? contentType
            : "application/octet-stream";
    }
}

using Domain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.Extensions;

public static class ResultExtensions
{
    /// <summary>
    /// Converts a <see cref="Result{T}"/> into the correct <see cref="IActionResult"/> so controllers
    /// never need their own if/switch on success or error codes.
    /// <para>
    /// The full result object (<c>IsSuccess</c> + <c>Value</c>/<c>Error</c>) is always the response body,
    /// giving clients a consistent shape regardless of outcome.
    /// </para>
    /// <para>
    /// Usage in a controller action: <c>return result.ToHttpResponse(this);</c>
    /// </para>
    /// </summary>
    public static IActionResult ToHttpResponse<T>(this Result<T> result, ControllerBase controller) =>
        result.IsSuccess
            ? controller.Ok(result)
            : result.Error!.Code switch
            {
                ErrorCode.NotFound     => controller.NotFound(result),
                ErrorCode.Conflict     => controller.Conflict(result),
                ErrorCode.Validation   => controller.BadRequest(result),
                ErrorCode.Unauthorized => new ObjectResult(result) { StatusCode = StatusCodes.Status401Unauthorized },
                ErrorCode.Forbidden    => new ObjectResult(result) { StatusCode = StatusCodes.Status403Forbidden },
                _                      => new ObjectResult(result) { StatusCode = StatusCodes.Status500InternalServerError },
            };
}

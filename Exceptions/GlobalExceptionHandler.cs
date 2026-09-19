using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace TiendaPromElec.Exceptions;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = exception switch
        {
            ResourceNotFoundException => (
                StatusCodes.Status404NotFound,
                "Resource not found",
                exception.Message),
            ConflictException => (
                StatusCodes.Status409Conflict,
                "Request conflict",
                exception.Message),
            DbUpdateException => (
                StatusCodes.Status409Conflict,
                "Database conflict",
                "The requested operation conflicts with the current data."),
            ArgumentException => (
                StatusCodes.Status400BadRequest,
                "Invalid request",
                exception.Message),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Unexpected error",
                "An unexpected error occurred while processing the request.")
        };

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception for request {TraceId}", httpContext.TraceIdentifier);
        }
        else
        {
            logger.LogWarning(exception, "Request failed with status {StatusCode}", statusCode);
        }

        await Results.Problem(
            statusCode: statusCode,
            title: title,
            detail: detail,
            instance: httpContext.Request.Path,
            extensions: new Dictionary<string, object?>
            {
                ["traceId"] = httpContext.TraceIdentifier
            }).ExecuteAsync(httpContext);
        return true;
    }
}

using Microsoft.AspNetCore.Diagnostics;

namespace dotnet_auth.Middlewares;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var statusCode = ResolveStatusCode(exception);

        logger.LogError(exception,
            "Error: {ExceptionType} | {Message} | Path: {Path}",
            exception.GetType().Name,
            exception.Message,
            httpContext.Request.Path);

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(new
        {
            success = false,
            error = exception.Message ?? "Internal Server Error",
            timestamp = DateTime.UtcNow.ToString("O")
        }, cancellationToken);

        return true;
    }

    private static int ResolveStatusCode(Exception exception)
    {
        return exception switch
        {
            AppException ex => ex.StatusCode, // custom app exception
            UnauthorizedAccessException => 401,
            ArgumentException
                or ArgumentNullException => 400,
            KeyNotFoundException => 404,
            OperationCanceledException => 499,
            _ => 500
        };
    }
}
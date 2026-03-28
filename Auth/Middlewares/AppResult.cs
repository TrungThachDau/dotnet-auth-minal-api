namespace dotnet_auth.Middlewares;

public static class AppResult
{
    private static IResult Json(object body, int statusCode) =>
        Results.Json(body, statusCode: statusCode);

    private static object Body(string? message, object? data) => new
    {
        success = true,
        message,
        data,
        timestamp = DateTime.UtcNow.ToString("O")
    };

    // 200 OK
    public static IResult Ok(string? message = "OK", object? data = null) =>
        Json(Body(message, data), 200);

    // 201 Created
    public static IResult Created(string? message = "Created", object? data = null) =>
        Json(Body(message, data), 201);

    // 202 Accepted
    public static IResult Accepted(string? message = "Accepted", object? data = null) =>
        Json(Body(message, data), 202);

    // 204 No Content
    public static IResult NoContent() =>
        Results.NoContent();
}

namespace dotnet_auth.Middlewares;


public class AppException(string message, int statusCode = 500) : Exception(message)
{
    public int StatusCode { get; } = statusCode;

    // Factory methods tiện dụng
    public static AppException BadRequest(string message)
    {
        return new AppException(message, 400);
    }

    public static AppException Unauthorized(string message = "Unauthorized")
    {
        return new AppException(message, 401);
    }

    public static AppException Forbidden(string message = "Forbidden")
    {
        return new AppException(message, 403);
    }

    public static AppException NotFound(string message)
    {
        return new AppException(message, 404);
    }

    public static AppException Internal(string message = "Internal Server Error")
    {
        return new AppException(message);
    }
}
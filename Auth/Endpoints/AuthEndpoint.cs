using dotnet_auth.Contracts.Auth;
using dotnet_auth.Middlewares;
using dotnet_auth.Services;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_auth.Endpoints;

public static class AuthEndpoint
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var config = app.ServiceProvider.GetRequiredService<IConfiguration>();
        var tokenLifetimeMinutes = int.TryParse(config["Jwt:ExpiresMinutes"], out var m) ? m : 60;

        app.MapGet("/auth", () => Results.Ok(new
        {
            status = "Server is running.",
            time = DateTime.UtcNow.AddMinutes(tokenLifetimeMinutes)
        })).WithName("HealthCheck");

        static bool IsInvalidLogin(SignInRequest? login)
        {
            return login is null
                   || string.IsNullOrWhiteSpace(login.Password)
                   || (string.IsNullOrWhiteSpace(login.Username) && string.IsNullOrWhiteSpace(login.Email));
        }

        async Task<IResult> HandleSignIn([FromServices] IAuthService authService, HttpContext http,
            [FromBody] SignInRequest? login)
        {
            if (IsInvalidLogin(login))
                return Results.BadRequest(new
                {
                    message = "Payload invalid. Required: password and either username or email."
                });

            var result = await authService.SignInAsync(login!);

            if (!result.Success)
                throw AppException.Unauthorized("Invalid credentials");

            http.Response.Cookies.Append("jwt", result.Token!, new CookieOptions
            {
                HttpOnly = true,
                Secure = http.Request.IsHttps,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(tokenLifetimeMinutes)
            });

            return AppResult.Ok(
                "Sign-in success");
        }

        app.MapPost("/auth/sign-in", HandleSignIn).WithName("SignIn");

        IResult HandleSignOut([FromServices] IAuthService authService, HttpContext http)
        {
            authService.SignOutAsync();
            http.Response.Cookies.Delete("jwt");
            return AppResult.Ok("Sign-out success");
        }

        app.MapPost("/auth/sign-out", HandleSignOut).WithName("SignOut").RequireAuthorization();


        app.MapPost("/auth/sign-up",
            async ([FromServices] IAuthService authService, [FromBody] RegisterRequest register) =>
            {
                var result = await authService.RegisterAsync(register.Username, register.Email, register.Password);

                return !result.Success
                    ? Results.BadRequest(
                        new { message = result.ErrorMessage })
                    : Results.Ok(new
                    {
                        message = "User registered successfully"
                    });
            }).WithName("Register");

        app.MapGet("/users", async ([FromServices] IAuthService authService)
                => await authService.GetAllUsersAsync())
            .WithName("GetAllUsers")
            .RequireAuthorization();
    }

    private record RegisterRequest(string Username, string Email, string Password);
}
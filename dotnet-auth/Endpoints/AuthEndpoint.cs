using dotnet_auth.Services;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_auth.Endpoints
{
    public static class AuthEndpoint
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var config = app.ServiceProvider.GetRequiredService<IConfiguration>();
            var tokenLifetimeMinutes = int.TryParse(config["Jwt:ExpiresMinutes"], out var m) ? m : 60;

            app.MapGet("/", () => Results.Ok(new
            {
                status = "Server is running.",
                time = DateTime.UtcNow.AddMinutes(tokenLifetimeMinutes)
            })).WithName("HealthCheck");
            app.MapPost("/auth/sign-in", async ([FromServices] IAuthService authService, HttpContext http, [FromBody] LoginRequest login) =>
            {
                var result = await authService.SignInAsync(login.Username, login.Password);

                if (!result.Success)
                {
                    return Results.Unauthorized();
                }

                http.Response.Cookies.Append("jwt", result.Token!, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(tokenLifetimeMinutes)
                });

                return Results.Ok(new { token = result.Token });
            }).WithName("SignIn");

            app.MapPost("/auth/sign-up", async ([FromServices] IAuthService authService, [FromBody] RegisterRequest register) =>
            {
                var result = await authService.RegisterAsync(register.Username, register.Password);

                return !result.Success ? Results.BadRequest(
                    new { message = result.ErrorMessage }) :
                    Results.Ok(new
                    {
                        message = "User registered successfully"
                    });
            }).WithName("Register");

            app.MapGet("/users", async ([FromServices] IAuthService authService)
                => await authService.GetAllUsersAsync())
                .WithName("GetAllUsers")
                .RequireAuthorization();
        }

        private record LoginRequest(string Username, string Password);

        private record RegisterRequest(string Username, string Email, string Password);
    }
}


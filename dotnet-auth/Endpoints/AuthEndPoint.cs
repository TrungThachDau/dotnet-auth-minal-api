using dotnet_auth.Services;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_auth.Endpoints
{
  public static class AuthEndPoint
  {
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
      var config = app.ServiceProvider.GetRequiredService<IConfiguration>();
      var tokenLifetimeMinutes = int.TryParse(config["Jwt:ExpiresMinutes"], out var m) ? m : 60;

      app.MapPost("/auth/sign-in", async ([FromServices] IAuthService authService, HttpContext http, [FromBody] LoginRequest login) =>
      {
        var token = await authService.SignInAsync(login.Username, login.Password);
        if (string.IsNullOrEmpty(token)) return Results.Unauthorized();

        http.Response.Cookies.Append("jwt", token, new CookieOptions
        {
          HttpOnly = true,
          Secure = true,
          SameSite = SameSiteMode.Strict,
          Expires = DateTimeOffset.UtcNow.AddMinutes(tokenLifetimeMinutes)
        });

        return Results.Ok(new { token });
      }).WithName("SignIn");

      app.MapPost("/auth/register", async ([FromServices] IAuthService authService, [FromBody] RegisterRequest register) =>
      {
        var success = await authService.RegisterAsync(register.Username, register.Email, register.Password);
        if (!success)
        {
          return Results.BadRequest(new { message = "User already exists" });
        }

        return Results.Ok(new { message = "User registered successfully" });
      }).WithName("Register");

      app.MapGet("/users", async ([FromServices] IAuthService authService) =>
      {
        return await authService.GetAllUsersAsync();
      }).WithName("GetAllUsers").RequireAuthorization();
    }

    public record LoginRequest(string Username, string Password);
    public record RegisterRequest(string Username, string Email, string Password);
  }
}


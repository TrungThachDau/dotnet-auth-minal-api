using dotnet_auth.Services;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace dotnet_auth.Endpoints
{
  public static class AuthEndPoint
  {
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
      app.MapPost("/auth/sign-in", async ([FromServices] IAuthService authService, HttpContext http, [FromBody] LoginRequest login) =>
      {
        var token = await authService.SignInAsync(login.Username, login.Password);
        if (string.IsNullOrEmpty(token)) return Results.Unauthorized();

        http.Response.Cookies.Append("jwt", token, new CookieOptions
        {
          HttpOnly = true,
          Secure = true,
          SameSite = SameSiteMode.Strict,
          Expires = DateTimeOffset.UtcNow.AddHours(1)
        });

        return Results.Ok(new { token });
      }).WithName("SignIn");

      // ...existing code...
      app.MapGet("/users", async ([FromServices] IAuthService authService) =>
      {
        return await authService.GetAllUsersAsync();
      }).WithName("GetAllUsers");
    }

    // Định nghĩa record cho request body (đặt ngoài method)
    public record LoginRequest(string Username, string Password);
  }
}


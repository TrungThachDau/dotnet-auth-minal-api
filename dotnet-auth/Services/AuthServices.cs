
using Microsoft.EntityFrameworkCore;
using dotnet_auth.Models;
using Microsoft.Extensions.Configuration;

namespace dotnet_auth.Services
{
  public class AuthService : IAuthService
  {
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext context, IConfiguration configuration)
    {
      _context = context;
      _configuration = configuration;
    }


    public Task<string?> SignInAsync(string username, string password)
    {
      // TODO: Validate username & password with DB
      // Dummy: always success
      var isValid = true;
      if (!isValid) return Task.FromResult<string?>(null);

      // JWT claims
      var claims = new[]
      {
          new System.Security.Claims.Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, username),
          new System.Security.Claims.Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
      };

      // Get secret from config
      var secret = _configuration["Jwt:Secret"] ?? "2a+S0N1gEls4FnqjZbBYjdEHzXp9oqTLUpoxZcZiZE0=";
      var issuer = _configuration["Jwt:Issuer"] ?? "android17x.com";
      var audience = _configuration["Jwt:Audience"] ?? "android17x.com";

      var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret));
      var creds = new Microsoft.IdentityModel.Tokens.SigningCredentials(key, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);

      var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
          issuer: issuer,
          audience: audience,
          claims: claims,
          expires: DateTime.UtcNow.AddHours(1),
          signingCredentials: creds
      );

      var tokenString = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
      return Task.FromResult<string?>(tokenString);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
      return await _context.Users.ToListAsync();
    }

  }
}
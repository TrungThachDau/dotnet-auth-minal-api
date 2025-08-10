
using Microsoft.EntityFrameworkCore;
using dotnet_auth.Models;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace dotnet_auth.Services
{
  public class AuthService : IAuthService
  {
    private readonly AppDbContext _context;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly SigningCredentials _signingCreds;
    private readonly TimeSpan _tokenLifetime;
    private static readonly JwtSecurityTokenHandler _tokenHandler = new();

    public AuthService(AppDbContext context, IConfiguration configuration)
    {
      _context = context;

      // Cache config & signing materials to avoid re-reading/allocating each call
      var secret = configuration["Jwt:Secret"] ?? "2a+S0N1gEls4FnqjZbBYjdEHzXp9oqTLUpoxZcZiZE0=";
      _issuer = configuration["Jwt:Issuer"] ?? "android17x.com";
      _audience = configuration["Jwt:Audience"] ?? "android17x.com";

      // If your secret is Base64, replace with Convert.FromBase64String(secret)
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
      _signingCreds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var lifetimeMinutes = int.TryParse(configuration["Jwt:ExpiresMinutes"], out var m)
        ? m
        : 60; // default 60 minutes
      _tokenLifetime = TimeSpan.FromMinutes(lifetimeMinutes);
    }


    public async Task<string?> SignInAsync(string username, string password)
    {
      // Fast-fail invalid input
      if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
      {
        return null;
      }

      // Query minimal fields, no tracking for read-only
      var user = await _context.Users
        .AsNoTracking()
        .Where(u => u.Name == username)
        .Select(u => new { u.Id, u.Name, u.Email, u.Password })
        .SingleOrDefaultAsync();

      // TODO: Replace plain-text comparison with hashed password verification
      if (user is null || user.Password != password)
      {
        return null;
      }

      var now = DateTime.UtcNow;
      var claims = new List<Claim>
      {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
        new Claim(ClaimTypes.Name, user.Name)
      };
      if (!string.IsNullOrWhiteSpace(user.Email))
      {
        claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
      }

      var token = new JwtSecurityToken(
        issuer: _issuer,
        audience: _audience,
        claims: claims,
        notBefore: now,
        expires: now.Add(_tokenLifetime),
        signingCredentials: _signingCreds
      );

      var tokenString = _tokenHandler.WriteToken(token);
      return tokenString;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
      return await _context.Users.ToListAsync();
    }

  }
}
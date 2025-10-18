
using Microsoft.EntityFrameworkCore;
using dotnet_auth.Models;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

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


        public async Task<AuthResult> SignInAsync(string username, string password)
        {
            // Fast-fail invalid input
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return new AuthResult(false, null, "Invalid username or password");
            }

            // Query minimal fields, no tracking for read-only
            var user = await _context.Users
              .AsNoTracking()
              .Where(u => u.Name == username)
              .Select(u => new { u.Id, u.Name, u.Email, u.Password })
              .SingleOrDefaultAsync();

            // Verify hashed password using BCrypt
            if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return new AuthResult(false, null, "Invalid username or password");
            }

            // Generate JWT token for authenticated user
            var token = GenerateJwtToken(user.Id, user.Name, user.Email);
            return new AuthResult(true, token, null);
        }

        private string GenerateJwtToken(string userId, string userName, string? userEmail)
        {
            var now = DateTime.UtcNow;
            var claims = new List<Claim>
      {
        new Claim(JwtRegisteredClaimNames.Sub, userId),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
        new Claim(ClaimTypes.Name, userName)
      };

            if (!string.IsNullOrWhiteSpace(userEmail))
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Email, userEmail));
            }

            var token = new JwtSecurityToken(
              issuer: _issuer,
              audience: _audience,
              claims: claims,
              notBefore: now,
              expires: now.Add(_tokenLifetime),
              signingCredentials: _signingCreds
            );

            return _tokenHandler.WriteToken(token);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<RegisterResult> RegisterAsync(string username, string email, string password)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return new RegisterResult(false, "Invalid input data");
            }

            // Check if user already exists
            var existingUser = await _context.Users
              .AsNoTracking()
              .AnyAsync(u => u.Name == username || u.Email == email);

            if (existingUser)
            {
                return new RegisterResult(false, "User already exists");
            }

            // Hash the password using BCrypt with work factor 12
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);

            var newUser = new User
            {
                Id = Guid.NewGuid().ToString(),
                Name = username,
                Email = email,
                Password = hashedPassword,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return new RegisterResult(true, null);
        }

    }
}
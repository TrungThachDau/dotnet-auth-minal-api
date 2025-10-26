using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using dotnet_auth.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace dotnet_auth.Services;

public class AuthService(AppDbContext context, JwtSettings jwtSettings) : IAuthService
{
    private static readonly JwtSecurityTokenHandler _tokenHandler = new();

    private readonly SigningCredentials _signingCreds = new(
        jwtSettings.GetSymmetricSecurityKey(),
        SecurityAlgorithms.HmacSha256);


    public async Task<AuthResult> SignInAsync(string username, string password)
    {
        // Fast-fail invalid input
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return new AuthResult(false, null, "Invalid username or password");

        // Query minimal fields, no tracking for read-only
        var user = await context.Users
            .AsNoTracking()
            .Where(u => u.username == username)
            .Select(u => new { Id = u.id, u.name, u.username, u.password })
            .SingleOrDefaultAsync();

        // Verify hashed password using BCrypt
        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.password))
            return new AuthResult(false, null, "Invalid username or password");

        // Generate JWT token for authenticated user
        var token = GenerateJwtToken(user.Id.ToString(), user.username);
        return new AuthResult(true, token, null);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await context.Users.ToListAsync();
    }

    public async Task<RegisterResult> RegisterAsync(string username, string password)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(username)  ||
            string.IsNullOrWhiteSpace(password)) return new RegisterResult(false, "Invalid input data");

        // Check if user already exists
        var existingUser = await context.Users
            .AsNoTracking()
            .AnyAsync(u => u.username == username);

        if (existingUser) return new RegisterResult(false, "User already exists");

        // Hash the password using BCrypt with work factor 12
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, 12);

        var newUser = new User
        {
            username = username,
            password = hashedPassword,
            CreatedAt = DateTime.UtcNow
        };

        context.Users.Add(newUser);
        await context.SaveChangesAsync();

        return new RegisterResult(true, null);
    }

    private string GenerateJwtToken(string userId, string userName)
    {
        var now = DateTime.UtcNow;
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64),
            new(ClaimTypes.Name, userName)
        };

        if (!string.IsNullOrWhiteSpace(userName)) claims.Add(new Claim(JwtRegisteredClaimNames.Email, userName));

        var token = new JwtSecurityToken(
            jwtSettings.Issuer,
            jwtSettings.Audience,
            claims,
            now,
            now.AddMinutes(jwtSettings.ExpiresMinutes),
            _signingCreds
        );

        return _tokenHandler.WriteToken(token);
    }
}
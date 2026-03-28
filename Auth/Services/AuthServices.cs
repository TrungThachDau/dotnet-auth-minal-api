using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using dotnet_auth.Contracts.Auth;
using dotnet_auth.Data;
using dotnet_auth.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace dotnet_auth.Services;

public class AuthService(AppDbContext context, JwtSettings jwtSettings) : IAuthService
{
    private const int BcryptWorkFactor = 12;
    private const string InvalidCredentialsMessage = "Invalid username or password";
    private const string InvalidInputMessage = "Invalid input data";
    private const string UserExistsMessage = "User already exists";
    private static readonly JwtSecurityTokenHandler TokenHandler = new();

    private readonly SigningCredentials _signingCreds = new(
        jwtSettings.GetSymmetricSecurityKey(),
        SecurityAlgorithms.HmacSha256);

    public async Task<AuthResult> SignInAsync(SignInRequest request)
    {
        var validationError = ValidateSignInRequest(request);
        if (validationError is not null)
            return new AuthResult(false, null, validationError);

        var identifier = DetermineIdentifier(request);
        if (identifier is null)
            return new AuthResult(false, null, InvalidCredentialsMessage);

        var user = await FindUserByIdentifierAsync(identifier);
        if (user is null)
            return new AuthResult(false, null, InvalidCredentialsMessage);

        if (!VerifyPassword(request.Password, user.password))
            return new AuthResult(false, null, InvalidCredentialsMessage);

        var token = GenerateJwtToken(user.Id.ToString(), user.username, user.email);
        return new AuthResult(true, token, null);
    }

    public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
    {
        return await context.Users
            .AsNoTracking()
            .Select(user => new UserResponse(
                user.id,
                user.username,
                user.name,
                user.email,
                user.CreatedAt))
            .ToListAsync();
    }

    public async Task<RegisterResult> RegisterAsync(string username, string email, string password)
    {
        var validationError = ValidateRegistrationInput(username, email, password);
        if (validationError is not null)
            return new RegisterResult(false, validationError);

        if (await UserExistsAsync(username, email))
            return new RegisterResult(false, UserExistsMessage);

        var hashedPassword = HashPassword(password);
        var newUser = CreateUser(username, email, hashedPassword);

        context.Users.Add(newUser);
        await context.SaveChangesAsync();

        return new RegisterResult(true, null);
    }

    public Task SignOutAsync()
    {
        return Task.CompletedTask;
    }

    private record UserLoginInfo(int Id, string username, string email, string password);

    #region Private Helper Methods

    private static string? ValidateSignInRequest(SignInRequest? request)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Password))
            return InvalidCredentialsMessage;

        return null;
    }

    private static string? DetermineIdentifier(SignInRequest request)
    {
        var identifier = string.IsNullOrWhiteSpace(request.Username)
            ? request.Email
            : request.Username;

        return string.IsNullOrWhiteSpace(identifier)
            ? null
            : identifier.Trim();
    }

    private async Task<UserLoginInfo?> FindUserByIdentifierAsync(string identifier)
    {
        return await context.Users
            .AsNoTracking()
            .Where(u => u.username == identifier
                        || u.email == identifier
                        || u.name == identifier)
            .Select(u => new UserLoginInfo(u.id, u.username, u.email, u.password))
            .FirstOrDefaultAsync();
    }

    private static bool VerifyPassword(string plainTextPassword, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(plainTextPassword, hashedPassword);
    }

    private static string? ValidateRegistrationInput(string username, string email, string password)
    {
        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
            return InvalidInputMessage;

        return null;
    }

    private async Task<bool> UserExistsAsync(string username, string email)
    {
        Console.WriteLine("Test Githook");
        return await context.Users
            .AsNoTracking()
            .AnyAsync(u => u.username == username || u.email == email);
    }

    private static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, BcryptWorkFactor);
    }

    private static User CreateUser(string username, string email, string hashedPassword)
    {
        return new User
        {
            username = username,
            name = username,
            email = email,
            password = hashedPassword,
            CreatedAt = DateTime.UtcNow
        };
    }

    private string GenerateJwtToken(string userId, string userName, string email)
    {
        var now = DateTime.UtcNow;
        var claims = BuildClaims(userId, userName, email, now);

        var token = new JwtSecurityToken(
            jwtSettings.Issuer,
            jwtSettings.Audience,
            claims,
            now,
            now.AddMinutes(jwtSettings.ExpiresMinutes),
            _signingCreds
        );

        return TokenHandler.WriteToken(token);
    }

    private static IEnumerable<Claim> BuildClaims(string userId, string userName, string email, DateTime issuedAt)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(issuedAt).ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64),
            new(ClaimTypes.Name, userName)
        };

        if (!string.IsNullOrWhiteSpace(email)) claims.Add(new Claim(JwtRegisteredClaimNames.Email, email));

        return claims;
    }

    #endregion
}

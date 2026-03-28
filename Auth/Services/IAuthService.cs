using dotnet_auth.Contracts.Auth;
namespace dotnet_auth.Services;

public interface IAuthService
{
    Task<AuthResult> SignInAsync(SignInRequest request);
    Task<RegisterResult> RegisterAsync(string username, string email, string password);
    Task<IEnumerable<UserResponse>> GetAllUsersAsync();

    Task SignOutAsync();
}

public record AuthResult(bool Success, string? Token, string? ErrorMessage);

public record RegisterResult(bool Success, string? ErrorMessage);



using dotnet_auth.Models;

namespace dotnet_auth.Services
{
    public interface IAuthService
    {
        Task<AuthResult> SignInAsync(string username, string password);
        Task<RegisterResult> RegisterAsync(string username, string email, string password);
        Task<IEnumerable<User>> GetAllUsersAsync();
    }

    public record AuthResult(bool Success, string? Token, string? ErrorMessage);
    public record RegisterResult(bool Success, string? ErrorMessage);
}
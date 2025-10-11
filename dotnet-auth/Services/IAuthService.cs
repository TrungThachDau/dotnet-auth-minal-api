

using dotnet_auth.Models;

namespace dotnet_auth.Services
{
  public interface IAuthService
  {
    Task<string?> SignInAsync(string username, string password);
    Task<bool> RegisterAsync(string username, string email, string password);
    //Get all users
    Task<IEnumerable<User>> GetAllUsersAsync();

  }
}
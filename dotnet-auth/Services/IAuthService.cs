



namespace.dotnet_auth.Services
{
  public interface IAuthService
  {
    Task<bool> SignInAsync(string username, string password);
    //Get all users
    Task<IEnumerable<User>> GetAllUsersAsync();
    
  }
}
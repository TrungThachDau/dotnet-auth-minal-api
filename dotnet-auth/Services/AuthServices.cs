


namespace dotnet_auth.Services
{
  public class AuthService(AppDbContext context) : IAuthService
  {
     private readonly IConfiguration _configuration = configuration;
    public async Task<bool> SignInAsync(string username, string password)
    { 
     return await context.Users.ToListAsync();
    }
  
  }
}
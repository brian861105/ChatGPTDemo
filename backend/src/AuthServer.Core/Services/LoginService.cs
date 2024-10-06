using AuthServer.Core.Interface;
namespace AuthServer.Core.Services;

public class LoginService : ILoginService
{
    // private readonly LoginServerDbContext _context;
    public static char[] SecretKey { get; set; }

    public async Task<bool> AuthenticateAsync(string username, string password)
    {
        throw new NotImplementedException();
    }

    public async Task<string> GenerateJwtTokenAsync(string username)
    {
        throw new NotImplementedException();
    }
}

using AuthServer.Core.Interface;
using AuthServer.Infrastructure.Data.Migrations;

namespace AuthServer.Core.Services;

public class LoginService(AuthDbContext context) : ILoginService
{
    private AuthDbContext _context = context;

    public async Task<bool> AuthenticateAsync(string username, string password)
    {
        var user = _context.Users.Where(x => x.Username == username).FirstOrDefault();
        if (Cryptography.VerifyHash(password, user.PasswordHash))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<string> GenerateJwtTokenAsync(string username)
    {
        throw new NotImplementedException();
    }
}

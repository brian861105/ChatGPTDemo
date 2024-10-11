using AuthServer.Core.Interface;
using AuthServer.Infrastructure.Data.Migrations;
using AuthServer.Core.Model;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthServer.Infrastructure.Services;

namespace AuthServer.Core.Services;

public class LoginService(AuthDbContext context, IOptions<LoginServiceOptions> options) : ILoginService
{
    private AuthDbContext _context = context;
    private readonly LoginServiceOptions Options = options.Value;

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
        var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(Options.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, username)
        };

        var token = new JwtSecurityToken(
            issuer: Options.Issuer,
            audience: Options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(Options.ExpirationHours),
            signingCredentials: credentials
        );

        return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }
}

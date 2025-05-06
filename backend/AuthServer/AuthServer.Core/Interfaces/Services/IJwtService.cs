using System.Security.Claims;
using AuthServer.Domain.Entity;

namespace AuthServer.Core.Interfaces.Services;

public interface IJwtService
{
    (string AccessToken, string RefreshToken) GenerateTokens(User user);
    string FreshToken(string token);
    bool ValidateToken(string token);
    ClaimsPrincipal GetPrincipalFromToken(string token);
}
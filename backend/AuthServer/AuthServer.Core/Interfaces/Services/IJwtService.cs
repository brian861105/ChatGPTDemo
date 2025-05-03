using System.Security.Claims;
using AuthServer.Domain.Entity;

namespace AuthServer.Core.Interfaces.Services;

public interface IJwtService
{
    string GenerateToken(User user);
    bool ValidateToken(string token);
    ClaimsPrincipal GetPrincipalFromToken(string token);
}
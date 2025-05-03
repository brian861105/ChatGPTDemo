using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthServer.Core.Interfaces.Services;
using AuthServer.Core.Settings;
using AuthServer.Domain.Entity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthServer.Infrastructure.Services;

public class JwtService(IOptions<JwtSettings> jwtSettings) : IJwtService
{
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new();
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    public string GenerateToken(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user), "User cannot be null.");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Email),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var key = new SymmetricSecurityKey(Convert.FromBase64String(_jwtSettings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_jwtSettings.ExpiryMinutes),
            signingCredentials: creds
        );

        return _jwtSecurityTokenHandler.WriteToken(token);
    }

    public bool ValidateToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        var key = new SymmetricSecurityKey(Convert.FromBase64String(_jwtSettings.SecretKey));
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = _jwtSettings.ValidateIssuer,
            ValidIssuer = _jwtSettings.Issuer,
            ValidateAudience = _jwtSettings.ValidateAudience,
            ValidAudience = _jwtSettings.Audience,
            ValidateLifetime = _jwtSettings.ValidateLifetime,
            ValidateIssuerSigningKey = _jwtSettings.ValidateIssuerSigningKey,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            _jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out _);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public ClaimsPrincipal GetPrincipalFromToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Token cannot be null or empty.", nameof(token));
        }

        var key = new SymmetricSecurityKey(Convert.FromBase64String(_jwtSettings.SecretKey));
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = _jwtSettings.ValidateIssuer,
            ValidIssuer = _jwtSettings.Issuer,
            ValidateAudience = _jwtSettings.ValidateAudience,
            ValidAudience = _jwtSettings.Audience,
            ValidateLifetime = _jwtSettings.ValidateLifetime,
            ValidateIssuerSigningKey = _jwtSettings.ValidateIssuerSigningKey,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var principal = _jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out _);
            return principal;
        }
        catch (Exception ex)
        {
            throw new SecurityTokenException("Invalid token.", ex);
        }
    }
}
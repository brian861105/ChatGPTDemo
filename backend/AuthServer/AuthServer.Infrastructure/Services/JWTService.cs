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

    public (string AccessToken, string RefreshToken) GenerateTokens(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user), "User cannot be null.");
        }

        // Generate Access Token
        var accessTokenClaims = new[]
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Email),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var accessTokenKey = new SymmetricSecurityKey(Convert.FromBase64String(_jwtSettings.AccessSecret));
        var accessTokenCreds = new SigningCredentials(accessTokenKey, SecurityAlgorithms.HmacSha256);

        var accessTokenJwt = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: accessTokenClaims,
            expires: DateTime.Now.AddMinutes(_jwtSettings.ExpiryMinutes),
            signingCredentials: accessTokenCreds
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(accessTokenJwt);

        var refreshKey = GenerateRefreshToken(user.Email);
        return (accessToken, refreshKey);
    }

    public string FreshToken(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new ArgumentException("Refresh token cannot be null or empty.", nameof(refreshToken));
        }

        // Extract email from refresh token to validate it
        var principal = GetPrincipalFromRefreshToken(refreshToken);
        var email = principal.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email) || !ValidateFreshToken(refreshToken, email))
        {
            throw new SecurityTokenException("Invalid refresh token.");
        }

        // Generate new access token
        var accessTokenClaims = new[]
        {
            new Claim(ClaimTypes.Name, email),
            new Claim(ClaimTypes.NameIdentifier, email),
            new Claim(ClaimTypes.Email, email)
        };

        var accessTokenKey = new SymmetricSecurityKey(Convert.FromBase64String(_jwtSettings.AccessSecret));
        var accessTokenCreds = new SigningCredentials(accessTokenKey, SecurityAlgorithms.HmacSha256);

        var accessTokenJwt = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: accessTokenClaims,
            expires: DateTime.Now.AddMinutes(_jwtSettings.ExpiryMinutes),
            signingCredentials: accessTokenCreds
        );

        return _jwtSecurityTokenHandler.WriteToken(accessTokenJwt);
    }

    public bool ValidateToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        var key = new SymmetricSecurityKey(Convert.FromBase64String(_jwtSettings.AccessSecret));
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

        var key = new SymmetricSecurityKey(Convert.FromBase64String(_jwtSettings.AccessSecret));
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

    private ClaimsPrincipal GetPrincipalFromRefreshToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Token cannot be null or empty.", nameof(token));
        }

        var key = new SymmetricSecurityKey(Convert.FromBase64String(_jwtSettings.RefreshSecret));
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = _jwtSettings.ValidateIssuer,
            ValidIssuer = _jwtSettings.Issuer,
            ValidateAudience = _jwtSettings.ValidateAudience,
            ValidAudience = _jwtSettings.Audience,
            ValidateLifetime = false,
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
            throw new SecurityTokenException("Invalid refresh token.", ex);
        }
    }

    private string GenerateRefreshToken(string userEmail)
    {
        var refreshTokenClaims = new[]
        {
            new Claim(ClaimTypes.Email, userEmail),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var refreshTokenKey = new SymmetricSecurityKey(Convert.FromBase64String(_jwtSettings.RefreshSecret));
        var refreshTokenCreds = new SigningCredentials(refreshTokenKey, SecurityAlgorithms.HmacSha256);

        var refreshTokenJwt = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: refreshTokenClaims,
            expires: DateTime.Now.AddDays(_jwtSettings.RefreshTokenExpiryDays), // 一天的過期時間
            signingCredentials: refreshTokenCreds
        );

        return _jwtSecurityTokenHandler.WriteToken(refreshTokenJwt);
    }

    private bool ValidateFreshToken(string token, string email)
    {
        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
        {
            return false;
        }

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(_jwtSettings.RefreshSecret)),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // 不允許任何時間偏差
            };

            // 驗證令牌並獲取主體聲明
            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            // 確認令牌類型
            if (!(validatedToken is JwtSecurityToken jwtToken) ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            // 驗證令牌中的電子郵件聲明是否與提供的電子郵件匹配
            var emailClaim = principal.FindFirst(ClaimTypes.Email)?.Value;
            if (emailClaim != email)
            {
                return false;
            }

            return true;
        }
        catch (Exception)
        {
            // 令牌驗證失敗（過期、簽名無效等）
            return false;
        }
    }
}
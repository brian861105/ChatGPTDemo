using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AuthServer.Core.Services;

namespace EntrySystemDemoUnitTest;
[TestFixture]
public class LoginServiceTests
{
    private LoginService _loginService;

    [SetUp]
    public void Setup()
    {
        _loginService = new LoginService();
    }

    [Test]
    public async Task AuthenticateAsync_ValidCredentials_ReturnsTrue()
    {
        // Arrange
        string username = "testUser";
        string password = "correctPassword";

        // Act
        bool result = await _loginService.AuthenticateAsync(username, password);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task AuthenticateAsync_InvalidCredentials_ReturnsFalse()
    {
        // Arrange
        string username = "testUser";
        string password = "wrongPassword";

        // Act
        bool result = await _loginService.AuthenticateAsync(username, password);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task GenerateJwtTokenAsync_ValidUsername_ReturnsValidToken()
    {
        // Arrange
        string username = "testUser";

        // Act
        string token = await _loginService.GenerateJwtTokenAsync(username);

        // Assert
        Assert.That(token, Is.Not.Null);
        Assert.That(IsValidJwtToken(token), Is.True);
        Assert.That(GetUsernameFromToken(token), Does.Contain(username));
    }

    private bool IsValidJwtToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(LoginService.SecretKey);
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private string GetUsernameFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        return jwtToken.Claims.First(claim => claim.Type == "username").Value;
    }
}
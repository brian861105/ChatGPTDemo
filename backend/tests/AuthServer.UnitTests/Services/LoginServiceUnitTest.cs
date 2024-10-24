using System.IdentityModel.Tokens.Jwt;
using System.Text;
using AuthServer.Core.Interface;
using AuthServer.Core.Model;
using AuthServer.Core.Services;
using AuthServer.Infrastructure.Data.Migrations;
using AuthServer.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace AuthServer.UnitTests.Services;

[TestFixture]
public class LoginServiceUnitTest
{
    private Mock<AuthDbContext> _mockContext;
    private Mock<DbSet<AuthUser>> _mockUserDbSet;
    private ILoginService _loginService;
    private static IOptions<LoginServiceOptions> loginServiceOptions;

    [SetUp]
    public void Setup()
    {
        // 使用 Factory 創建 mock context
        (_mockContext, _mockUserDbSet) = MockDbContextFactory.CreateMockAuthDbContext();

        var loginService = new LoginServiceOptions
        {
            SecretKey = "F3YiY8jbVUqgB4TiMyAZcJnLBQeXyAr9AYTN53yxwBc",
            Issuer = "testIssuer",
            Audience = "testAudience",
            ExpirationHours = 1
        };
        loginServiceOptions = Options.Create(loginService);
        _loginService = new LoginService(_mockContext.Object, loginServiceOptions);
    }
    [Test]
    public async Task AuthenticateAsync_ValidCredentials_ReturnsTrue()
    {
        // Arrange
        string username = "test1";
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
        string username = "test1";
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

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(token, Is.Not.Null);
            Assert.That(IsValidJwtToken(token), Is.True);
            Assert.That(GetUsernameFromToken(token), Is.EqualTo(username));
        });
    }


    private static IOptions<LoginServiceOptions> CreateLoginServiceOptions(LoginServiceOptions loginServiceOptions)
    {
        return Options.Create(loginServiceOptions);
    }

    private static bool IsValidJwtToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(loginServiceOptions.Value.SecretKey);
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

    private static string GetUsernameFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        return jwtToken.Claims.First(claim => claim.Type == "sub").Value;
    }
}

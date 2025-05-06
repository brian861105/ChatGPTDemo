using AuthServer.Core.Interfaces.Services;
using AuthServer.Core.Settings;
using AuthServer.Domain.Entity;
using AuthServer.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthServer.Tests.Services;

[TestFixture]
[TestOf(typeof(JwtService))]
public class JwtServiceTest
{
    [SetUp]
    public void Setup()
    {
        _jwtSettings = new JwtSettings
        {
            AccessSecret = "876091d2b6de57771b1dec152da8d2867004922d2ac5c7bb6a76fb4443a9af5f",
            RefreshSecret = "c2538c7b88b37166231f1ff8b815fb4f0b3cf242dc38dfbf77ba186e1e255640",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpiryMinutes = 15,
            RefreshTokenExpiryDays = 7,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
        var options = Options.Create(_jwtSettings);
        _jwtService = new JwtService(options);
        _passwordService = new PasswordService();
    }

    private PasswordService _passwordService;
    private IJwtService _jwtService;
    private JwtSettings _jwtSettings;

    [Test]
    public void GenerateToken_ValidUser_ReturnsToken()
    {
        // Arrange
        var user = new User
        (
            "test@example.com",
            _passwordService.HashPassword("hashedpassword")
        );

        // Act
        var (acceToken, refreshToken) = _jwtService.GenerateTokens(user);

        // Assert
        Assert.That(acceToken, Is.Not.Null);
        Assert.That(acceToken, Is.Not.Empty);
        Assert.That(refreshToken, Is.Not.Null);
        Assert.That(refreshToken, Is.Not.Empty);
    }

    [Test]
    public void ValidateAccessToken_ReturnsTrue()
    {
        // Arrange
        var user = new User
        (
            "test@example.com",
            _passwordService.HashPassword("hashedpassword")
        );

        var (accessToken, refreshToken) = _jwtService.GenerateTokens(user);

        // Act
        var isValid = _jwtService.ValidateToken(accessToken);

        // Assert
        Assert.That(isValid, Is.True);
    }

    [Test]
    public void ValidateToken_InvalidToken_ReturnsFalse()
    {
        // Arrange
        const string invalidToken = "invalid.token.string";

        // Act
        var isValid = _jwtService.ValidateToken(invalidToken);

        // Assert
        Assert.That(isValid, Is.False);
    }

    [Test]
    public void ValidateToken_ExpiredToken_ReturnsFalse()
    {
        // Arrange
        _jwtSettings.ExpiryMinutes = -1;
        var options = Options.Create(_jwtSettings);
        _jwtService = new JwtService(options);

        var user = new User
        (
            "test@example.com",
            _passwordService.HashPassword("hashedpassword")
        );

        var (accessToken, refreshToken) = _jwtService.GenerateTokens(user);

        // Act
        var isValid = _jwtService.ValidateToken(accessToken);

        // Assert
        Assert.That(isValid, Is.False);
    }

    [Test]
    public void FreshToken_ValidToken_ReturnsNewAccessToken()
    {
        // Arrange
        var user = new User
        (
            "test@example.com",
            _passwordService.HashPassword("hashedpassword")
        );

        var (_, refreshToken) = _jwtService.GenerateTokens(user);
        var accessToken = _jwtService.FreshToken(refreshToken);

        // Act
        var isValid = _jwtService.ValidateToken(accessToken);

        // Assert
        Assert.That(isValid, Is.True);
    }

    [Test]
    public void FreshToken_InvalidToken_ThrowsException()
    {
        // Arrange 
        var invalidToken = "";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _jwtService.FreshToken(invalidToken));
    }

    [Test]
    public void FreshToken_ExpiredToken_ThrowsException()
    {
        // Arrange
        _jwtSettings.RefreshTokenExpiryDays = -1;
        var options = Options.Create(_jwtSettings);
        _jwtService = new JwtService(options);

        var user = new User
        (
            "test@example.com",
            _passwordService.HashPassword("hashedpassword")
        );

        var (_, refreshToken) = _jwtService.GenerateTokens(user);

        // Act & Assert
        Assert.Throws<SecurityTokenException>(() => _jwtService.FreshToken(refreshToken));
    }

    [Test]
    public void GetPrincipalFromToken_ValidToken_ReturnsClaimsPrincipal()
    {
        // Arrange
        var user = new User
        (
            "test@example.com",
            _passwordService.HashPassword("hashedpassword")
        );
        var (accessToken, refreshToken) = _jwtService.GenerateTokens(user);

        // Act
        var principal = _jwtService.GetPrincipalFromToken(accessToken);

        // Assert
        Assert.That(principal, Is.Not.Null);
        Assert.That(principal.Identity?.Name, Is.EqualTo(user.Email));
    }

    [Test]
    public void GetPrincipalFromToken_InvalidToken_ThrowsException()
    {
        // Arrange
        const string invalidToken = "invalid.token.string";

        // Act & Assert
        Assert.Throws<SecurityTokenException>(() => _jwtService.GetPrincipalFromToken(invalidToken));
    }

    [TestCase("")]
    public void ValidateToken_EmptyOrNullToken_ReturnsFalse(string token)
    {
        // Act
        var isValid = _jwtService.ValidateToken(token);

        // Assert
        Assert.That(isValid, Is.False);
    }

    [TestCase("")]
    public void GetPrincipalFromToken_EmptyOrNullToken_ThrowsArgumentException(string token)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _jwtService.GetPrincipalFromToken(token));
    }

    [Test]
    public void GenerateToken_NullUser_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _jwtService.GenerateTokens(null));
    }
}
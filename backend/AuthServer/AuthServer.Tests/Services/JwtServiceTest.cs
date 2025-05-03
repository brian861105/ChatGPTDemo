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
            SecretKey = "876091d2b6de57771b1dec152da8d2867004922d2ac5c7bb6a76fb4443a9af5f",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpiryMinutes = 15,
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
        var token = _jwtService.GenerateToken(user);

        // Assert
        Assert.That(token, Is.Not.Null);
        Assert.That(token, Is.Not.Empty);
    }

    [Test]
    public void ValidateToken_ValidToken_ReturnsTrue()
    {
        // Arrange
        var user = new User
        (
            "test@example.com",
            _passwordService.HashPassword("hashedpassword")
        );

        var token = _jwtService.GenerateToken(user);

        // Act
        var isValid = _jwtService.ValidateToken(token);

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

        var token = _jwtService.GenerateToken(user);

        // Act
        var isValid = _jwtService.ValidateToken(token);

        // Assert
        Assert.IsFalse(isValid);
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
        var token = _jwtService.GenerateToken(user);

        // Act
        var principal = _jwtService.GetPrincipalFromToken(token);

        // Assert
        Assert.That(principal, Is.Not.Null);
        Assert.That(principal.Identity?.Name, Is.EqualTo(user.Email));
    }

    [Test]
    public void GetPrincipalFromToken_InvalidToken_ThrowsException()
    {
        // Arrange
        var invalidToken = "invalid.token.string";

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
        Assert.Throws<ArgumentNullException>(() => _jwtService.GenerateToken(null));
    }
}
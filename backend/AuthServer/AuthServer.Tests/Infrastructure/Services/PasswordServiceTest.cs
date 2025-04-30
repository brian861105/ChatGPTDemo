using AuthServer.Infrastructure.Services;

namespace AuthServer.Tests.Infrastructure.Services;

[TestFixture]
[TestOf(typeof(PasswordService))]
public class PasswordServiceTest
{
    private PasswordService _passwordService;

    [SetUp]
    public void SetUp()
    {
        _passwordService = new PasswordService();
    }

    [Test]
    public void HashPassword_ValidPassword_ReturnsValidHash()
    {
        // Arrange
        const string password = "password";
    
        // Act
        var hashedPassword = _passwordService.HashPassword(password);
    
        // Assert
        Assert.That(hashedPassword, Is.Not.Null.And.Not.Empty);
        Assert.That(_passwordService.VerifyPassword(password, hashedPassword), Is.True);
    }

    [Test]
    [TestCase("")]
    [TestCase("    ")]
    public void HashPassword_InvalidPassword_ThrowsException(string invalidPassword)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _passwordService.HashPassword(invalidPassword));
    }

    [Test]
    [TestCase("", "securePassword")]
    [TestCase(" ", "securePassword")]
    public void VerifyPassword_InvalidInput_ThrowsException(string incorrectPassword, string password)
    {
        // Arrange
        var hashedPassword = _passwordService.HashPassword(password);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _passwordService.VerifyPassword(incorrectPassword, hashedPassword));
    }
}
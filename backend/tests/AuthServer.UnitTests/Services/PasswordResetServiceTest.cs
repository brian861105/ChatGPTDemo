using Moq;
using AuthServer.Core.Interface;

namespace AuthServer.UnitTests.Services;

[TestFixture]
public class PasswordResetServiceTests
{
    private readonly IPasswordResetService _PasswordResetService;

    public PasswordResetServiceTests()
    {
        _PasswordResetService = new MockPasswordResetService();
    }

    [Test]
    public async Task InitiatePasswordResetAsync_ValidEmail_ReturnsTrue()
    {
        // Arrange
        // Act
        var result = await _PasswordResetService.InitiatePasswordResetAsync("valid@email.com");

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task ValidateResetTokenAsync_ValidToken_ReturnsTrue()
    {
        // Arrange
        // Act
        var result = await _PasswordResetService.ValidateResetTokenAsync("valid@email.com", "validToken");

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task ResetPasswordAsync_ValidTokenAndPassword_ReturnsTrue()
    {
        // Arrange
        // Act
        var result = await _PasswordResetService.ResetPasswordAsync("valid@email.com", "validToken", "newPassword123");

        // Assert
        Assert.That(result, Is.True);
    }
}
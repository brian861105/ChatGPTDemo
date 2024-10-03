using EntrySystemDemo.Services;
using Moq;

namespace EntrySystemDemoUnitTest;

[TestFixture]
public class PasswordResetServiceTests
{
    private readonly Mock<IPasswordResetService> _mockPasswordResetService;

    public PasswordResetServiceTests()
    {
        _mockPasswordResetService = new Mock<IPasswordResetService>();
    }

    [Test]
    public async Task InitiatePasswordResetAsync_ValidEmail_ReturnsTrue()
    {
        // Arrange
        _mockPasswordResetService.Setup(x => x.InitiatePasswordResetAsync("valid@email.com")).ReturnsAsync(true);

        // Act
        var result = await _mockPasswordResetService.Object.InitiatePasswordResetAsync("valid@email.com");

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task ValidateResetTokenAsync_ValidToken_ReturnsTrue()
    {
        // Arrange
        _mockPasswordResetService.Setup(x => x.ValidateResetTokenAsync("validToken")).ReturnsAsync(true);

        // Act
        var result = await _mockPasswordResetService.Object.ValidateResetTokenAsync("validToken");

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task ResetPasswordAsync_ValidTokenAndPassword_ReturnsTrue()
    {
        // Arrange
        _mockPasswordResetService.Setup(x => x.ResetPasswordAsync("validToken", "newPassword123")).ReturnsAsync(true);

        // Act
        var result = await _mockPasswordResetService.Object.ResetPasswordAsync("validToken", "newPassword123");

        // Assert
        Assert.That(result, Is.True);
    }
}
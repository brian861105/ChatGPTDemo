using Moq;
using AuthServer.Core.Interface;
using Microsoft.Extensions.Logging;
using AuthServer.Infrastructure.Data.Migrations;

namespace AuthServer.UnitTests.Services;

[TestFixture]
public class PasswordResetServiceTests
{
    private readonly IPasswordResetService _PasswordResetService;
    private Mock<ILogger<IPasswordResetService>> _logger;
    private List<AuthUser> _testUsers;

    public PasswordResetServiceTests()
    {
        _PasswordResetService = new MockPasswordResetService();
        _logger = new Mock<ILogger<IPasswordResetService>>();
    }
    [Test]
    public async Task InitiatePasswordReset_EmptyEmail_ReturnsFalse()
    {
        // Arrange

        string email = "";

        // Act
        var result = await _PasswordResetService.InitiatePasswordResetAsync(email);

        // Assert
        Assert.That(result, Is.False);
    }
    [Test]
    public async Task InitiatePasswordResetAsync_ValidEmail_ReturnsTrue()
    {
        // Arrange
        // Act
        var result = await _PasswordResetService.InitiatePasswordResetAsync(TestConstants.ValidEmail);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task ValidateResetTokenAsync_ValidToken_ReturnsTrue()
    {
        // Arrange
        await _PasswordResetService.InitiatePasswordResetAsync(TestConstants.ValidEmail);
        // Act
        var result = await _PasswordResetService.ValidateResetTokenAsync(TestConstants.ValidEmail, TestConstants.ValidToken);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task ResetPasswordAsync_ValidTokenAndPassword_ReturnsTrue()
    {
        // Arrange
        await _PasswordResetService.InitiatePasswordResetAsync(TestConstants.ValidEmail);
        // Act
        var result = await _PasswordResetService.ResetPasswordAsync(TestConstants.ValidEmail, TestConstants.ValidToken, TestConstants.ValidPassword);

        // Assert
        Assert.That(result, Is.True);
    }
}
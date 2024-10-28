using AuthServer.Core.Interface;
using AuthServer.Core.Services;
using AuthServer.Infrastructure.Data.Migrations;
using AuthServer.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace AuthServer.UnitTests.Services;
[TestFixture]
public class RegistrationServiceTests
{
    private IRegistrationService registrationService;

    [SetUp]
    public void Setup()
    {
        registrationService = new MockRegistrationService();
    }

    [Test]
    public async Task RegisterUserAsync_ValidInput_ReturnsTrue()
    {
        // Act
        var result = await registrationService.RegisterUserAsync("testuser", "test@example.com", "password123");

        // Assert
        Assert.That(result.Success, Is.True);
    }

    [Test]
    public async Task RegisterUserAsync_DuplicateUsername_ReturnsFalse()
    {
        // Arrange
        await registrationService.RegisterUserAsync("testuser", "test@example.com", "password123");

        // Act
        var result = await registrationService.RegisterUserAsync("testuser", "another@example.com", "password456");

        // Assert
        Assert.That(result.Success, Is.False);
    }

    [Test]
    public async Task RegisterUserAsync_DuplicateEmail_ReturnsFalse()
    {
        // Arrange
        await registrationService.RegisterUserAsync("testuser1", "test@example.com", "password123");

        // Act
        var result = await registrationService.RegisterUserAsync("testuser2", "test@example.com", "password456");

        // Assert
        Assert.That(result.Success, Is.False);
    }

    [Test]
    public async Task IsUsernameAvailableAsync_AvailableUsername_ReturnsTrue()
    {
        // Act
        var result = await registrationService.IsUsernameAvailableAsync("newuser");

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task IsUsernameAvailableAsync_UnavailableUsername_ReturnsFalse()
    {
        // Arrange
        await registrationService.RegisterUserAsync("testuser", "test@example.com", "password123");

        // Act
        var result = await registrationService.IsUsernameAvailableAsync("testuser");

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task IsEmailAvailableAsync_AvailableEmail_ReturnsTrue()
    {
        // Act
        var result = await registrationService.IsEmailAvailableAsync("new@example.com");

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task IsEmailAvailableAsync_UnavailableEmail_ReturnsFalse()
    {
        // Arrange
        await registrationService.RegisterUserAsync("testuser", "test@example.com", "password123");

        // Act
        var result = await registrationService.IsEmailAvailableAsync("test@example.com");

        // Assert
        Assert.That(result, Is.False);
    }
}
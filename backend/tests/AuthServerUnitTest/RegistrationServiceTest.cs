using System.Threading.Tasks;
using AuthServer.Core.Interface;
using EntrySystemDemo.Services;
using NUnit.Framework;

namespace AuthServerUnitTest;
[TestFixture]
public class RegistrationServiceTests
{
    private IRegistrationService _service;

    [SetUp]
    public void Setup()
    {
        _service = new RegistrationService();
    }

    [Test]
    public async Task RegisterUserAsync_ValidInput_ReturnsTrue()
    {
        // Act
        var result = await _service.RegisterUserAsync("testuser", "test@example.com", "password123");

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task RegisterUserAsync_DuplicateUsername_ReturnsFalse()
    {
        // Arrange
        await _service.RegisterUserAsync("testuser", "test@example.com", "password123");

        // Act
        var result = await _service.RegisterUserAsync("testuser", "another@example.com", "password456");

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task RegisterUserAsync_DuplicateEmail_ReturnsFalse()
    {
        // Arrange
        await _service.RegisterUserAsync("testuser1", "test@example.com", "password123");

        // Act
        var result = await _service.RegisterUserAsync("testuser2", "test@example.com", "password456");

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task IsUsernameAvailableAsync_AvailableUsername_ReturnsTrue()
    {
        // Act
        var result = await _service.IsUsernameAvailableAsync("newuser");

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task IsUsernameAvailableAsync_UnavailableUsername_ReturnsFalse()
    {
        // Arrange
        await _service.RegisterUserAsync("testuser", "test@example.com", "password123");

        // Act
        var result = await _service.IsUsernameAvailableAsync("testuser");

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task IsEmailAvailableAsync_AvailableEmail_ReturnsTrue()
    {
        // Act
        var result = await _service.IsEmailAvailableAsync("new@example.com");

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task IsEmailAvailableAsync_UnavailableEmail_ReturnsFalse()
    {
        // Arrange
        await _service.RegisterUserAsync("testuser", "test@example.com", "password123");

        // Act
        var result = await _service.IsEmailAvailableAsync("test@example.com");

        // Assert
        Assert.That(result, Is.False);
    }
}
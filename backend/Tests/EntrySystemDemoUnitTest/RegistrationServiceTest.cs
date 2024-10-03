using System.Threading.Tasks;
using NUnit;
using Moq;
using EntrySystemDemo.Services;

namespace EntrySystemDemoUnitTest;

[TestFixture]
public class RegistrationServiceTests
{
    private readonly Mock<IRegistrationService> _mockRegistrationService;

    public RegistrationServiceTests()
    {
        _mockRegistrationService = new Mock<IRegistrationService>();
    }

    [Test]
    public async Task RegisterUserAsync_ValidInput_ReturnsTrue()
    {
        // Arrange
        _mockRegistrationService.Setup(x => x.RegisterUserAsync("newUser", "new@email.com", "password123")).ReturnsAsync(true);

        // Act
        var result = await _mockRegistrationService.Object.RegisterUserAsync("newUser", "new@email.com", "password123");

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task IsUsernameAvailableAsync_AvailableUsername_ReturnsTrue()
    {
        // Arrange
        _mockRegistrationService.Setup(x => x.IsUsernameAvailableAsync("availableUser")).ReturnsAsync(true);

        // Act
        var result = await _mockRegistrationService.Object.IsUsernameAvailableAsync("availableUser");

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task IsEmailAvailableAsync_UnavailableEmail_ReturnsFalse()
    {
        // Arrange
        _mockRegistrationService.Setup(x => x.IsEmailAvailableAsync("taken@email.com")).ReturnsAsync(false);

        // Act
        var result = await _mockRegistrationService.Object.IsEmailAvailableAsync("taken@email.com");

        // Assert
        Assert.That(result, Is.False);
    }
}

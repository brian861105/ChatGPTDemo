using AuthServer.Core.Services;

namespace AuthServer.UnitTests.Validation;

[TestFixture]
public class RandomStringGeneratorTest
{
    [Test]
    public void GenerateRandomString_DefaultLength_ReturnsCorrectLength()
    {
        // Arrange
        int expectedLength = 10;

        // Act
        string result = RandomStringGenerator.GenerateRandomString(expectedLength);

        // Assert
        Assert.That(result.Length, Is.EqualTo(expectedLength));
    }

    [Test]
    public void GenerateRandomString_ZeroLength_ReturnsEmptyString()
    {
        // Act
        string result = RandomStringGenerator.GenerateRandomString(0);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GenerateRandomString_OnlyLowercase_ContainsOnlyLowercaseLetters()
    {
        // Arrange
        int length = 20;

        // Act
        string result = RandomStringGenerator.GenerateRandomString(length,
            includeLowercase: true,
            includeUppercase: false,
            includeNumbers: false);

        // Assert
        Assert.That(result, Does.Match("^[a-z]+$"));
        Assert.That(result.Length, Is.EqualTo(length));
    }

    [Test]
    public void GenerateRandomString_OnlyUppercase_ContainsOnlyUppercaseLetters()
    {
        // Arrange
        int length = 20;

        // Act
        string result = RandomStringGenerator.GenerateRandomString(length,
            includeLowercase: false,
            includeUppercase: true,
            includeNumbers: false);

        // Assert
        Assert.That(result, Does.Match("^[A-Z]+$"));
        Assert.That(result.Length, Is.EqualTo(length));
    }

    [Test]
    public void GenerateRandomString_OnlyNumbers_ContainsOnlyNumbers()
    {
        // Arrange
        int length = 20;

        // Act
        string result = RandomStringGenerator.GenerateRandomString(length,
            includeLowercase: false,
            includeUppercase: false,
            includeNumbers: true);

        // Assert
        Assert.That(result, Does.Match("^[0-9]+$"));
        Assert.That(result.Length, Is.EqualTo(length));
    }

    [Test]
    public void GenerateRandomString_AllCharacterTypes_ContainsAllTypes()
    {
        // Arrange
        int length = 1000; // 使用較長的字串以確保統計上的可靠性

        // Act
        string result = RandomStringGenerator.GenerateRandomString(length);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Does.Match("[a-z]"), "Should contain lowercase letters");
            Assert.That(result, Does.Match("[A-Z]"), "Should contain uppercase letters");
            Assert.That(result, Does.Match("[0-9]"), "Should contain numbers");
        });
    }

    [Test]
    public void GenerateRandomString_NoCharacterTypes_ThrowsArgumentException()
    {
        // Arrange & Act & Assert
        Assert.That(() => RandomStringGenerator.GenerateRandomString(10,
            includeLowercase: false,
            includeUppercase: false,
            includeNumbers: false),
            Throws.TypeOf<ArgumentException>());
    }

    [Test]
    public void GenerateRandomString_MultipleCalls_ReturnsDifferentResults()
    {
        // Arrange
        int length = 10;
        var results = new HashSet<string>();

        // Act
        for (int i = 0; i < 100; i++)
        {
            results.Add(RandomStringGenerator.GenerateRandomString(length));
        }

        // Assert
        // 由於是隨機生成，100次調用應該產生接近100個不同的結果
        Assert.That(results.Count, Is.GreaterThan(95), "Should generate mostly unique strings");
    }

    [Test]
    public void GenerateRandomString_LargeLength_HandlesCorrectly()
    {
        // Arrange
        int length = 10000;

        // Act
        string result = RandomStringGenerator.GenerateRandomString(length);

        // Assert
        Assert.That(result.Length, Is.EqualTo(length));
    }

    [Test]
    public void GenerateRandomString_NegativeLength_ThrowsArgumentException()
    {
        Assert.That(() => RandomStringGenerator.GenerateRandomString(-1),
            Throws.TypeOf<ArgumentException>()
                .With.Message.Contains("length"));
    }

    [TestCase(5, true, false, false)]
    [TestCase(5, false, true, false)]
    [TestCase(5, false, false, true)]
    [TestCase(10, true, true, true)]
    public void GenerateRandomString_VariousConfigurations_GeneratesValidStrings(
        int length, bool includeLower, bool includeUpper, bool includeNumbers)
    {
        // Act
        string result = RandomStringGenerator.GenerateRandomString(length,
            includeLower, includeUpper, includeNumbers);

        // Assert
        Assert.That(result.Length, Is.EqualTo(length));

        if (includeLower)
            Assert.That(result, Does.Match("[a-z]"));
        if (includeUpper)
            Assert.That(result, Does.Match("[A-Z]"));
        if (includeNumbers)
            Assert.That(result, Does.Match("[0-9]"));
    }
}
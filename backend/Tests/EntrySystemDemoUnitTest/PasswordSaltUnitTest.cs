using AuthServer.Core;

namespace EntrySystemDemoUnitTest;

public class PasswordCryptoUnitTest
{
    string password;
    [SetUp]
    public void Setup()
    {
        password = "testPassword";
    }

    [Test]
    public void TestEncryptedPasswordIsNotPlaintext()
    {
        var encrypted = Cryptography.Encrypto(password);
        Assert.That(password, Is.Not.EqualTo(encrypted));
    }

    [Test]
    public void TestSamePasswordGeneratesDifferentHashes()
    {
        var encrypted1 = Cryptography.Encrypto(password);
        var encrypted2 = Cryptography.Encrypto(password);
        Assert.That(encrypted1, Is.Not.EqualTo(encrypted2));
    }

    [Test]
    public void TestVerifyPassword()
    {
        var hash = Cryptography.Encrypto(password);

        bool isValid = Cryptography.VerifyHash(password, hash);
        Assert.That(isValid, Is.True);

        bool isInvalid = Cryptography.VerifyHash("wrongPassword", hash);
        Assert.That(isInvalid, Is.False);
    }
}
using AuthServer.Infrastructure.Data.Migrations;

namespace AuthServer.UnitTests.Services;

public static class TestConstants
{
    public const string Username = "testUser";
    public const string ValidEmail = "test@example.com";
    public const string ValidPassword = "Test123!@#";
    public const string ValidToken = "validToken123";
    public const int TokenExpirationMinutes = 3;
}

public static class TestDataFactory
{
    public static AuthUser CreateTestUser(
        string email = TestConstants.ValidEmail,
        string passwordHash = "hashedPassword")
    {
        return new AuthUser
        {
            Username = TestConstants.Username,
            Email = email,
            PasswordHash = passwordHash,
            ResetToken = null,
            ResetTokenExpires = null
        };
    }
}
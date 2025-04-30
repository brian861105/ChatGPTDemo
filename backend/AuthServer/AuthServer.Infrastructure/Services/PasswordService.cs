using AuthServer.Core.Interfaces;
using BCrypt.Net;
using AuthServer.Core.Interfaces.Services;

namespace AuthServer.Infrastructure.Services;

public class PasswordService: IPasswordService
{
    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password cannot be empty or contain only whitespace.", nameof(password));
        }
        var hash = BCrypt.Net.BCrypt.HashPassword(password);

        return hash;
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password cannot be empty or contain only whitespace.", nameof(password));
        }
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}
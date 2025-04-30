namespace AuthServer.Core.Interfaces.Services;

public partial interface IPasswordService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
}
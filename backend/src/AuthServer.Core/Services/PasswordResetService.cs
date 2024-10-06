using AuthServer.Core.Interface;
namespace AuthServer.Core.Services;

public class PasswordResetService : IPasswordResetService
{
    public Task<bool> InitiatePasswordResetAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ValidateResetTokenAsync(string token)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ResetPasswordAsync(string token, string newPassword)
    {
        throw new NotImplementedException();
    }
}

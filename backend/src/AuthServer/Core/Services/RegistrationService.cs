using AuthServer.Core.Interface;

namespace AuthServer.Core.Services;

public class RegistrationService : IRegistrationService
{
    Task<bool> IRegistrationService.IsEmailAvailableAsync(string email)
    {
        throw new NotImplementedException();
    }

    Task<bool> IRegistrationService.IsUsernameAvailableAsync(string username)
    {
        throw new NotImplementedException();
    }

    Task<(bool Success, string Message)> IRegistrationService.RegisterUserAsync(string username, string email, string password)
    {
        throw new NotImplementedException();
    }
}

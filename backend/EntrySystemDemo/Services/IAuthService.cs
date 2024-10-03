using System.Threading.Tasks;

namespace EntrySystemDemo.Services
{
    public interface ILoginService
    {
        Task<bool> AuthenticateAsync(string username, string password);
        Task<string> GenerateJwtTokenAsync(string username);
    }

    public interface IRegistrationService
    {
        Task<bool> RegisterUserAsync(string username, string email, string password);
        Task<bool> IsUsernameAvailableAsync(string username);
        Task<bool> IsEmailAvailableAsync(string email);
    }

    public interface IPasswordResetService
    {
        Task<bool> InitiatePasswordResetAsync(string email);
        Task<bool> ValidateResetTokenAsync(string token);
        Task<bool> ResetPasswordAsync(string token, string newPassword);
    }

    public interface IAuthService : ILoginService, IRegistrationService, IPasswordResetService
    {
    }
}
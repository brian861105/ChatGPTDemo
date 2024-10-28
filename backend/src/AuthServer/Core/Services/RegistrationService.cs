using AuthServer.Core.Interface;
using AuthServer.Core.Services;
using AuthServer.Infrastructure.Data;
using AuthServer.Infrastructure.Data.Migrations;
using AuthServer.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace AuthServer.Core.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly AuthDbContext _dbContext;

        public RegistrationService(AuthDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<(bool Success, string Message)> RegisterUserAsync(string username, string email, string password)
        {
            try
            {
                // Input validation
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    return (false, "所有欄位都必填");
                }

                // Check password strength
                if (!IsPasswordValid(password))
                {
                    return (false, "密碼不符合安全要求");
                }

                // Check availability
                if (!await IsUsernameAvailableAsync(username))
                {
                    return (false, "使用者名稱已被使用");
                }

                if (!await IsEmailAvailableAsync(email))
                {
                    return (false, "Email 已被註冊");
                }

                // Create new user with hashed password
                var newUser = new AuthUser
                {
                    Username = username.Trim(),
                    Email = email.ToLower().Trim(),
                    PasswordHash = Cryptography.Encrypto(password),
                    CreatedAt = DateTime.UtcNow
                };

                // Add user and save changes
                await _dbContext.Users.AddAsync(newUser);
                await _dbContext.SaveChangesAsync();

                return (true, "註冊成功");
            }
            catch (Exception ex)
            {
                // TODO: Add proper logging here
                return (false, $"註冊失敗: {ex.Message}");
            }
        }

        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return !await _dbContext.Users
                .AnyAsync(u => u.Email.Equals(email.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        public async Task<bool> IsUsernameAvailableAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            return !await _dbContext.Users
                .AnyAsync(u => u.Username.Equals(username.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        private bool IsPasswordValid(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            // 至少8個字元
            if (password.Length < 8)
                return false;

            // 至少包含一個大寫字母
            if (!password.Any(char.IsUpper))
                return false;

            // 至少包含一個小寫字母
            if (!password.Any(char.IsLower))
                return false;

            // 至少包含一個數字
            if (!password.Any(char.IsDigit))
                return false;

            // 至少包含一個特殊字元
            var specialCharacters = "!@#$%^&*()_+-=[]{}|;:,.<>?";
            if (!password.Any(c => specialCharacters.Contains(c)))
                return false;

            return true;
        }
    }
}
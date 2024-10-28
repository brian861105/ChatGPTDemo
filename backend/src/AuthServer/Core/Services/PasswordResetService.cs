using AuthServer.Core.Interface;
using AuthServer.Infrastructure.Data.Migrations;
using AuthServer.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AuthServer.Core.Services;
public class PasswordResetService(
    AuthDbContext dbContext,
    ILogger<PasswordResetService> logger) : IPasswordResetService
{
    private readonly AuthDbContext _dbContext = dbContext;
    private readonly ILogger<PasswordResetService> _logger = logger;

    public async Task<bool> InitiatePasswordResetAsync(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            _logger.LogWarning("Attempted password reset with empty email");
            return false;
        }

        try
        {
            var user = await FindUserByEmailAsync(email);
            if (user == null)
            {
                _logger.LogInformation("Password reset attempted for non-existent email: {Email}", email);
                return false;
            }

            user.ResetToken = await GenerateSecureTokenAsync();
            user.ResetTokenExpires = DateTime.UtcNow.AddMinutes(3);
            user.PasswordHash = null;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            await SendPasswordResetEmailAsync(user);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password reset initiation for email: {Email}", email);
            return false;
        }
    }

    public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
    {
        if (!await ValidateResetTokenAsync(email, token))
        {
            return false;
        }

        try
        {
            var user = await FindUserByEmailAsync(email);
            if (user == null) return false;

            if (!IsValidPassword(newPassword))
            {
                _logger.LogWarning("Invalid password format attempted for email: {Email}", email);
                return false;
            }

            user.PasswordHash = Cryptography.Encrypto(newPassword);
            user.ResetToken = null;
            user.ResetTokenExpires = null;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password reset for email: {Email}", email);
            return false;
        }
    }

    public async Task<bool> ValidateResetTokenAsync(string email, string token)
    {
        if (string.IsNullOrEmpty(token)) return false;

        try
        {
            var user = await FindUserByEmailAsync(email);
            if (user == null) return false;

            if (user.ResetTokenExpires < DateTime.UtcNow)
            {
                _logger.LogInformation("Reset token expired for email: {Email}", email);
                return false;
            }

            return user.ResetToken == token;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating reset token for email: {Email}", email);
            return false;
        }
    }

    private async Task<AuthUser?> FindUserByEmailAsync(string email)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower().Trim());
    }

    private async Task<string> GenerateSecureTokenAsync()
    {
        // 生成安全的重置令牌
        var randomNumber = new byte[32];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return await Task.FromResult(Convert.ToBase64String(randomNumber));
    }

    private bool IsValidPassword(string password)
    {
        // 實作密碼複雜度要求
        if (string.IsNullOrEmpty(password)) return false;
        if (password.Length < 8) return false;

        bool hasNumber = password.Any(char.IsDigit);
        bool hasUpper = password.Any(char.IsUpper);
        bool hasLower = password.Any(char.IsLower);
        bool hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

        return hasNumber && hasUpper && hasLower && hasSpecial;
    }

    private async Task SendPasswordResetEmailAsync(AuthUser user)
    {
        await Task.CompletedTask;
    }
}
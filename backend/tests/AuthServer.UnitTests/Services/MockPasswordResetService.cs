using AuthServer.Core.Interface;
using AuthServer.Core.Services;
using AuthServer.Infrastructure.Data.Migrations;
using AuthServer.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace AuthServer.UnitTests.Services;

public class MockPasswordResetService : IPasswordResetService
{
    private readonly Mock<AuthDbContext> _dbContext;
    private readonly Mock<DbSet<AuthUser>> _userDbSet;
    private readonly List<AuthUser> _users;
    public IEnumerable<AuthUser> GetUsers() => _users;
    public void ClearUsers() => _users.Clear();
    public MockPasswordResetService()
    : this([]) // 使用預設空集合
    {
        (_dbContext, _userDbSet) = MockDbContextFactory.CreateMockAuthDbContext();
        _users = [.. _userDbSet.Object];  // 直接轉換成 List
    }
    public MockPasswordResetService(IEnumerable<AuthUser> customUsers)
    {
        (_dbContext, _userDbSet) = MockDbContextFactory.CreateMockAuthDbContext(customUsers);
        _users = [.. _userDbSet.Object.AsQueryable()];  // 直接轉換成 List
    }

    public async Task<bool> InitiatePasswordResetAsync(string email)
    {
        if (string.IsNullOrEmpty(email)) return false;

        var user = FindUserByEmail(email);
        if (user == null) return false;

        user.PasswordHash = null;
        user.ResetToken = "validToken";

        return await UpdateUserAsync(user);
    }

    public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
    {
        if (!await ValidateResetTokenAsync(email, token))
        {
            return false;
        }

        var user = FindUserByEmail(email);
        if (user == null) return false;

        user.PasswordHash = Cryptography.Encrypto(newPassword);
        user.ResetToken = null;

        return await UpdateUserAsync(user);
    }

    public async Task<bool> ValidateResetTokenAsync(string email, string token)
    {
        if (string.IsNullOrEmpty(token)) return false;

        var user = FindUserByEmail(email);
        return await Task.FromResult(user?.ResetToken == token);
    }

    private async Task<bool> UpdateUserAsync(AuthUser user)
    {
        try
        {
            _userDbSet.Setup(m => m.Update(It.IsAny<AuthUser>()))
                .Callback<AuthUser>(updatedUser =>
                {
                    var existingUser = _users.FirstOrDefault(u => u.Email == updatedUser.Email);
                    if (existingUser != null)
                    {
                        existingUser.PasswordHash = updatedUser.PasswordHash;
                        existingUser.ResetToken = updatedUser.ResetToken;
                    }
                });

            _userDbSet.Object.Update(user);
            _dbContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            await _dbContext.Object.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private AuthUser? FindUserByEmail(string email)
    {
        return _users.FirstOrDefault(u =>
            u.Email.Equals(email.Trim(), StringComparison.OrdinalIgnoreCase));
    }
}

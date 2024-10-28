using AuthServer.Core.Interface;
using AuthServer.Infrastructure.Data.Migrations;
using AuthServer.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace AuthServer.UnitTests.Services;

public class MockPasswordResetService : IPasswordResetService
{
    private readonly Mock<AuthDbContext> _dbContext;
    private readonly Mock<DbSet<AuthUser>> _userDbSet;
    private IEnumerable<AuthUser> _users;
    public MockPasswordResetService()
    {
        (_dbContext, _userDbSet) = MockDbContextFactory.CreateMockAuthDbContext();
        _users = _userDbSet.Object.AsQueryable();
    }
    public MockPasswordResetService(IEnumerable<AuthUser> customUsers)
    {
        // Initialize with custom test data
        (_dbContext, _userDbSet) = MockDbContextFactory.CreateMockAuthDbContext(customUsers);
        _users = _userDbSet.Object.AsQueryable();
    }

    public async Task<bool> InitiatePasswordResetAsync(string email)
    {
        if (String.IsNullOrEmpty(email)) return false;

        var user = _users
            .FirstOrDefault(u => u.Email.ToLower() == email.ToLower().Trim());

        if (user == null) return await Task.FromResult(false);

        try
        {
            user.PasswordHash = null;
            user.ResetToken = null;

            _userDbSet.Setup(m => m.Update(It.IsAny<AuthUser>()))
                .Callback<AuthUser>(updatedUser =>
                {
                    var userList = _userDbSet.Object.AsQueryable().ToList() as List<AuthUser>;
                    var existingUser = userList?.FirstOrDefault(u => u.Email == updatedUser.Email);
                    if (existingUser != null)
                    {
                        existingUser.PasswordHash = updatedUser.PasswordHash;
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

    public Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ValidateResetTokenAsync(string email, string token)
    {
        if (String.IsNullOrEmpty(token)) return false;

        var user = _userDbSet.Object
            .AsQueryable()
            .FirstOrDefault(u => u.Email.ToLower() == email.ToLower().Trim());

        if (user == null) return await Task.FromResult(false);

        if (user.ResetToken == token)
        {
            return true;
        }
        return false;

    }
}

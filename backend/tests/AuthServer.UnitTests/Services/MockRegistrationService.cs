using AuthServer.Core.Interface;
using AuthServer.Core.Services;
using AuthServer.Infrastructure.Data.Migrations;
using AuthServer.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace AuthServer.UnitTests.Services
{
    public class MockRegistrationService : IRegistrationService
    {
        private readonly Mock<AuthDbContext> _dbContext;
        private readonly Mock<DbSet<AuthUser>> _userDbSet;
        private IEnumerable<AuthUser> _users;

        public MockRegistrationService()
        {
            // Initialize with default test data
            (_dbContext, _userDbSet) = MockDbContextFactory.CreateMockAuthDbContext();
            _users = _userDbSet.Object.AsQueryable();
        }

        public MockRegistrationService(IEnumerable<AuthUser> customUsers)
        {
            // Initialize with custom test data
            (_dbContext, _userDbSet) = MockDbContextFactory.CreateMockAuthDbContext(customUsers);
            _users = _userDbSet.Object.AsQueryable();
        }

        public async Task<(bool Success, string Message)> RegisterUserAsync(string username, string email, string password)
        {
            try
            {
                // Input validation
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    return (false, "All fields are required");
                }

                // Check password strength
                if (!IsPasswordValid(password))
                {
                    return (false, "Password does not meet security requirements");
                }

                // Check availability
                if (!await IsUsernameAvailableAsync(username))
                {
                    return (false, "Username is already taken");
                }

                if (!await IsEmailAvailableAsync(email))
                {
                    return (false, "Email is already registered");
                }

                // Create new user
                var newUser = new AuthUser
                {
                    Username = username.Trim(),
                    Email = email.ToLower().Trim(),
                    PasswordHash = Cryptography.Encrypto(password),
                    CreatedAt = DateTime.UtcNow
                };

                // Setup mock to track added user
                _userDbSet.Setup(m => m.Add(It.IsAny<AuthUser>()))
                    .Callback<AuthUser>(user =>
                    {
                        var userList = _userDbSet.Object.AsQueryable().ToList() as List<AuthUser>;
                        userList?.Add(user);
                        _users = userList ?? Enumerable.Empty<AuthUser>();
                    });

                // Add user and save changes
                _userDbSet.Object.Add(newUser);
                _dbContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(1);

                await _dbContext.Object.SaveChangesAsync();
                return (true, "Registration successful");
            }
            catch (Exception ex)
            {
                // Log the exception in real implementation
                return (false, "Registration failed: " + ex.Message);
            }
        }

        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var exists = _users.Any(u => u.Email != null &&
                u.Email.Equals(email.Trim(), StringComparison.OrdinalIgnoreCase));
            return await Task.FromResult(!exists);
        }

        public async Task<bool> IsUsernameAvailableAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            var exists = _users.Any(u => u.Username != null &&
                u.Username.Equals(username.Trim(), StringComparison.OrdinalIgnoreCase));
            return await Task.FromResult(!exists);
        }

        private bool IsPasswordValid(string password)
        {
            // Add your password validation logic here
            return !string.IsNullOrWhiteSpace(password) && password.Length >= 8;
        }
    }
}
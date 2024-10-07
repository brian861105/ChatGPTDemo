using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AuthServer.Core.Services;
using AuthServer.Infrastructure.Data.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Moq;
using NUnit.Framework;

namespace AuthServerUnitTest
{
    [TestFixture]
    public class LoginServiceTests
    {
        private Mock<IDbContextFactory<AuthDbContext>> _mockFactory;
        private Mock<AuthDbContext> _mockContext;
        private Mock<DbSet<AuthUser>> _mockUserDbSet;  // 添加這行
        private LoginService _loginService;


        [SetUp]
        public void Setup()
        {
            _mockUserDbSet = new Mock<DbSet<AuthUser>>();
            _mockContext = new Mock<AuthDbContext>();
            _mockFactory = new Mock<IDbContextFactory<AuthDbContext>>();

            _mockContext.Setup(m => m.Users).Returns(_mockUserDbSet.Object);
            _mockFactory.Setup(f => f.CreateDbContext()).Returns(_mockContext.Object);

            _loginService = new LoginService(_mockFactory.Object);
        }

        [Test]
        public async Task AuthenticateAsync_ValidCredentials_ReturnsTrue()
        {
            // Arrange
            string username = "testUser";
            string password = "correctPassword";
            var user = new AuthUser { Username = username, PasswordHash = BCrypt.Net.BCrypt.HashPassword(password) };

            // 模擬 FirstOrDefaultAsync 方法
            _mockUserDbSet.Setup(m => m.FirstOrDefaultAsync(It.IsAny<Expression<Func<AuthUser, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Expression<Func<AuthUser, bool>> predicate, CancellationToken token) =>
                    user);

            // Act
            bool result = await _loginService.AuthenticateAsync(username, password);

            // Assert
            Assert.That(result, Is.True);
        }

        // [Test]
        // public async Task AuthenticateAsync_InvalidCredentials_ReturnsFalse()
        // {
        //     // Arrange
        //     string username = "testUser";
        //     string password = "wrongPassword";
        //     var user = new AuthUser { Username = username, PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctPassword") };
        //     _mockUserDbSet.Setup(m => m.FindAsync(username)).ReturnsAsync(user);

        //     // Act
        //     bool result = await _loginService.AuthenticateAsync(username, password);

        //     // Assert
        //     Assert.That(result, Is.False);
        // }

        // [Test]
        // public async Task GenerateJwtTokenAsync_ValidUsername_ReturnsValidToken()
        // {
        //     // Arrange
        //     string username = "testUser";

        //     // Act
        //     string token = await _loginService.GenerateJwtTokenAsync(username);

        //     // Assert
        //     Assert.That(token, Is.Not.Null);
        //     Assert.That(IsValidJwtToken(token), Is.True);
        //     Assert.That(GetUsernameFromToken(token), Is.EqualTo(username));
        // }

        // private bool IsValidJwtToken(string token)
        // {
        //     var tokenHandler = new JwtSecurityTokenHandler();
        //     var key = Encoding.ASCII.GetBytes(LoginService.SecretKey);
        //     try
        //     {
        //         tokenHandler.ValidateToken(token, new TokenValidationParameters
        //         {
        //             ValidateIssuerSigningKey = true,
        //             IssuerSigningKey = new SymmetricSecurityKey(key),
        //             ValidateIssuer = false,
        //             ValidateAudience = false,
        //             ClockSkew = TimeSpan.Zero
        //         }, out SecurityToken validatedToken);
        //         return true;
        //     }
        //     catch
        //     {
        //         return false;
        //     }
        // }

        // private string GetUsernameFromToken(string token)
        // {
        //     var tokenHandler = new JwtSecurityTokenHandler();
        //     var jwtToken = tokenHandler.ReadJwtToken(token);
        //     return jwtToken.Claims.First(claim => claim.Type == "username").Value;
        // }
    }
}
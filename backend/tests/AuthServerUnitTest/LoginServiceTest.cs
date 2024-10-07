using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AuthServer.Core.Interface;
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
        private Mock<AuthDbContext> _mockContext;
        private Mock<DbSet<AuthUser>> _mockUserDbSet;  // 添加這行
        private ILoginService _loginService;


        [SetUp]
        public void Setup()
        {

            var data = new List<AuthUser>
            {
                new() { Username = "test1", PasswordHash = Cryptography.Encrypto("correctPassword")},
                new() { Username = "test2", PasswordHash = Cryptography.Encrypto("incorrectPassword")},
                new() { Username = "test3", PasswordHash = Cryptography.Encrypto("incorrectPassword")},
            }.AsQueryable();


            _mockUserDbSet = new Mock<DbSet<AuthUser>>();
            _mockUserDbSet.As<IQueryable<AuthUser>>().Setup(m => m.Provider).Returns(data.Provider);
            _mockUserDbSet.As<IQueryable<AuthUser>>().Setup(m => m.Expression).Returns(data.Expression);
            _mockUserDbSet.As<IQueryable<AuthUser>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _mockUserDbSet.As<IQueryable<AuthUser>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            _mockContext = new Mock<AuthDbContext>();

            _mockContext.Setup(m => m.Users).Returns(_mockUserDbSet.Object);

            _loginService = new LoginService(_mockContext.Object);
        }

        [Test]
        public async Task AuthenticateAsync_ValidCredentials_ReturnsTrue()
        {
            // Arrange
            string username = "test1";
            string password = "correctPassword";

            // Act
            bool result = await _loginService.AuthenticateAsync(username, password);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task AuthenticateAsync_InvalidCredentials_ReturnsFalse()
        {
            // Arrange
            string username = "test1";
            string password = "wrongPassword";

            // Act
            bool result = await _loginService.AuthenticateAsync(username, password);

            // Assert
            Assert.That(result, Is.False);
        }

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
using AuthServer.Core.Services;
using AuthServer.Infrastructure.Data.Migrations;
using AuthServer.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace AuthServer.UnitTests.Services;

public class MockDbContextFactory
{
    public static (Mock<AuthDbContext> mockContext, Mock<DbSet<T>> mockDbSet) CreateMockDbContext<T>(IEnumerable<T> dataList) where T : class
    {
        var data = dataList.AsQueryable();

        var mockDbSet = new Mock<DbSet<T>>();
        mockDbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
        mockDbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
        mockDbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
        mockDbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDatabase_{Guid.NewGuid()}")
            .Options;

        var mockContext = new Mock<AuthDbContext>(options);

        return (mockContext, mockDbSet);
    }

    public static (Mock<AuthDbContext> mockContext, Mock<DbSet<AuthUser>> mockDbSet) CreateMockAuthDbContext(IEnumerable<AuthUser>? users = null)
    {
        users ??=
            [
                new() { Username = "test1", PasswordHash = Cryptography.Encrypto("correctPassword"), Email ="valid@email.com"},
                new() { Username = "test2", PasswordHash = Cryptography.Encrypto("incorrectPassword"), Email = "avalid@email.com"},
                new() { Username = "test3", PasswordHash = Cryptography.Encrypto("incorrectPassword"), Email = "bvalid@email.com"},
            ];

        var (mockContext, mockDbSet) = CreateMockDbContext(users);
        mockContext.Setup(m => m.Users).Returns(mockDbSet.Object);

        return (mockContext, mockDbSet);
    }
}

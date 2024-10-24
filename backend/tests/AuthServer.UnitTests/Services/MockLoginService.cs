using AuthServer.Core.Interface;
using Moq;
using System.Threading.Tasks;

namespace AuthServer.UnitTests.Services;
public class MockLoginServiceFactory
{
    public static Mock<ILoginService> CreateMockLoginService(
        bool defaultAuthenticationResult = true,
        string defaultJwtToken = "mock.jwt.token")
    {
        var mockLoginService = new Mock<ILoginService>();

        // 設置 AuthenticateAsync 的預設行為
        mockLoginService
            .Setup(x => x.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(defaultAuthenticationResult);

        // 可以為特定的使用者名稱和密碼組合設置特定的回傳值
        mockLoginService
            .Setup(x => x.AuthenticateAsync("validUser", "correctPassword"))
            .ReturnsAsync(true);

        mockLoginService
            .Setup(x => x.AuthenticateAsync("invalidUser", It.IsAny<string>()))
            .ReturnsAsync(false);

        // 設置 GenerateJwtTokenAsync 的預設行為
        mockLoginService
            .Setup(x => x.GenerateJwtTokenAsync(It.IsAny<string>()))
            .ReturnsAsync(defaultJwtToken);

        // 可以為特定的使用者名稱設置特定的 JWT token
        mockLoginService
            .Setup(x => x.GenerateJwtTokenAsync("specialUser"))
            .ReturnsAsync("special.jwt.token");

        // 模擬錯誤情況
        mockLoginService
            .Setup(x => x.AuthenticateAsync("errorUser", It.IsAny<string>()))
            .ThrowsAsync(new Exception("Authentication error"));

        mockLoginService
            .Setup(x => x.GenerateJwtTokenAsync("errorUser"))
            .ThrowsAsync(new Exception("Token generation error"));

        return mockLoginService;
    }
}

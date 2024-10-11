using System.Dynamic;

namespace AuthServer.Core.Model;

public class LoginModel
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public class LoginServiceOptions
{
    public required string SecretKey { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required double ExpirationHours { get; set; }
}
namespace AuthServer.Core.DTOs;

public class ValidateToken
{
    public string Token { get; set; } = string.Empty;
}

public class ValidateResponse
{
    public bool IsValid { get; set; }
}
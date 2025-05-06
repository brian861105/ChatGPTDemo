namespace AuthServer.Core.Settings;

public class JwtSettings
{
    public required string AccessSecret { get; set; }
    public required string RefreshSecret { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public int ExpiryMinutes { get; set; }
    public bool ValidateIssuer { get; set; } = true;
    public bool ValidateAudience { get; set; } = true;
    public bool ValidateLifetime { get; set; } = true;
    public bool ValidateIssuerSigningKey { get; set; } = true;
    public int RefreshTokenExpiryDays { get; set; } = 7;
}
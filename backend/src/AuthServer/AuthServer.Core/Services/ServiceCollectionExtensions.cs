
using AuthServer.Core.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AuthServer.Core.Services;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<LoginServiceOptions>(
            configuration.GetSection("JwtSettings"));

        services.PostConfigure<LoginServiceOptions>(options =>
        {
            if (string.IsNullOrEmpty(options.SecretKey) || Encoding.UTF8.GetBytes(options.SecretKey).Length < 32)
            {
                var key = new byte[32];
                using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
                {
                    rng.GetBytes(key);
                }
                options.SecretKey = Convert.ToBase64String(key);
                // 注意：在實際生產環境中，您應該安全地存儲這個新生成的密鑰
            }
        });

        var serviceProvider = services.BuildServiceProvider();
        var jwtSettings = serviceProvider.GetRequiredService<IOptions<LoginServiceOptions>>().Value;

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
            };
        });
        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<LoginService>();
        // 添加其他服務...

        return services;
    }
}
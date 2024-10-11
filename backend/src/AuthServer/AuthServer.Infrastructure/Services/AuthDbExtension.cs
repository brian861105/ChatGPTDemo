using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthServer.Infrastructure.Services;
public static class AuthDbContextExtensions
{
    public static IServiceCollection AddAuthDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AuthDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("AuthServer")));

        return services;
    }
}

using ChatgptDemo.Core;
using NLog;
using NLog.Web;

public static class Program
{
    #region Basic Program.cs
    public static void Main(string[] args)
    {
        var logger = NLog.LogManager.Setup().LoadConfigurationFromFile();

        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration
            .AddJsonFile("settings.json", optional: true, reloadOnChange: true)
        .AddJsonFile($"settings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
        builder.Services.AddControllers();
        builder.Services.Configure<Settings>(builder.Configuration);
        builder.Host.UseDefaultServiceProvider(options =>
        {
            options.ValidateScopes = true;
            options.ValidateOnBuild = true;
        });
        builder.Host.UseNLog();
        #endregion Basic Program.cs
    }
}
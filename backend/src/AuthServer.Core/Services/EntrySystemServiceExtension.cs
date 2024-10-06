// using Microsoft.Extensions.DependencyInjection;
// using EntrySystemDemo.Services;

// namespace EntrySystemDemo.Services;


// public static class ServiceCollectionExtensions
// {
//     public static IServiceCollection AddEntrySystemServices(this IServiceCollection services)
//     {
//         // 在這裡添加所有與 EntrySystem 相關的服務
//         services.AddScoped<ILoginService, LoginService>();
//         services.AddScoped<IRegistrationService, RegistrationService>();
//         services.AddScoped<IPasswordResetService, PasswordResetService>();
//         // 如果使用組合服務：
//         // services.AddScoped<IAuthService, AuthService>();
//         // 你可以繼續添加更多服務
//         // services.AddScoped<IAnotherService, AnotherService>();
//         // services.AddTransient<IYetAnotherService, YetAnotherService>();

//         // 如果有需要配置的服務，你也可以在這裡進行配置
//         // services.Configure<SomeOptions>(options => { ... });

//         return services;
//     }
// }

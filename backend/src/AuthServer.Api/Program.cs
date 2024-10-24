using AuthServer.Core.Services;
using AuthServer.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// 添加配置
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

// 添加服務到容器
builder.Services.AddControllers();

// 添加授權服務
builder.Services.AddJwtAuthentication(builder.Configuration);

// 添加資料庫服務
builder.Services.AddAuthDbContext(builder.Configuration);

// 添加應用服務
builder.Services.AddApplicationServices();

// 添加授權
builder.Services.AddAuthorization();


// 添加 LoginService
builder.Services.AddScoped<LoginService>();

// 添加 Swagger/OpenAPI 支持
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

// 配置 HTTP 請求管道
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    _ = app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 啟用身份驗證和授權
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
using AuthServer.Domain.Entity;
using AuthServer.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthServer.Infrastructure.Data.Context;

public class AuthDbContext(DbContextOptions<AuthDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserProfile> Profiles { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 配置 AuthUser
        modelBuilder.Entity<User>(ConfigureAuthUser);

        // 配置 UserProfile
        modelBuilder.Entity<UserProfile>(ConfigureUserProfile);
    }

    private void ConfigureAuthUser(EntityTypeBuilder<User> builder)
    {
        // 設置 Email 唯一索引
        builder.HasIndex(u => u.Email).IsUnique();

        // 配置一對一關聯：用戶->個人資料
        builder.HasOne(u => u.Profile).WithOne(p => p.User).HasForeignKey<UserProfile>(p => p.UserId);
    }

    private void ConfigureUserProfile(EntityTypeBuilder<UserProfile> builder)
    {
        // 在此可以添加額外的配置
        builder.HasIndex(p => p.UserId).IsUnique();
    }

    // 添加後備配置方法
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            throw new InvalidOperationException(
                "The DbContextOptions are not configured. Please configure the options before using the context.");
        }
    }
}

// 設計時 DbContext 工廠
// 這個工廠用於在設計時（例如，遷移）創建 DbContext 實例
// 在這裡可以配置連線字串等設置
public class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
{
    public AuthDbContext CreateDbContext(string[] args)
    {
        try
        {
            // 找到專案根目錄
            var basePath = Directory.GetCurrentDirectory();
            Console.WriteLine($"Current Directory: {basePath}");

            // 讀取連線字串 (添加防錯處理)
            var connectionString = "Host=localhost;Port=5432;Database=chatgpt;Username=postgres;Password=postgres";
            // 如果找不到連線字串，使用默認值
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = "Host=localhost;Database=authdb;Username=postgres;Password=postgres";
                Console.WriteLine("警告: 無法從配置文件中讀取連接字串，使用默認值");
            }

            Console.WriteLine($"Connection String: {connectionString}");

            // 建立選項
            var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();
            optionsBuilder.UseNpgsql(connectionString,
                npgsqlOptions => npgsqlOptions.MigrationsAssembly("AuthServer.Infrastructure"));

            return new AuthDbContext(optionsBuilder.Options);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"創建 DbContext 時發生錯誤: {ex.Message}");
            throw;
        }
    }
}
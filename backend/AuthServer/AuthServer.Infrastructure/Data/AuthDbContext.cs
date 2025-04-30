using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using AuthServer.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthServer.Infrastructure.Data
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        public DbSet<AuthUser> Users { get; set; } = null!;
        public DbSet<UserProfile> Profiles { get; set; } = null!;
        public DbSet<AuthSession> Sessions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 配置 AuthUser
            modelBuilder.Entity<AuthUser>(ConfigureAuthUser);

            // 配置 UserProfile
            modelBuilder.Entity<UserProfile>(ConfigureUserProfile);

            // 配置 AuthSession
            modelBuilder.Entity<AuthSession>(ConfigureAuthSession);
        }

        private void ConfigureAuthUser(EntityTypeBuilder<AuthUser> builder)
        {
            // 設置 Email 唯一索引
            builder.HasIndex(u => u.Email).IsUnique();

            // 配置一對一關聯：用戶->個人資料
            builder.HasOne(u => u.Profile)
                .WithOne(p => p.User)
                .HasForeignKey<UserProfile>(p => p.UserId);

            // 配置一對多關聯：用戶->會話
            builder.HasMany(u => u.Sessions)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void ConfigureUserProfile(EntityTypeBuilder<UserProfile> builder)
        {
            // 在此可以添加額外的配置
            builder.HasIndex(p => p.UserId).IsUnique();
        }

        private void ConfigureAuthSession(EntityTypeBuilder<AuthSession> builder)
        {
            // 添加索引
            builder.HasIndex(s => s.Token);
            builder.HasIndex(s => s.UserId);
        }
        
        // 添加後備配置方法
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // 這只是後備方案，在設計時工廠失敗的情況下使用
                optionsBuilder.UseNpgsql("Host=localhost;Database=authdb;Username=postgres;Password=postgres",
                    npgsqlOptions => npgsqlOptions.MigrationsAssembly("AuthServer.Infrastructure"));
            }
        }
    }

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
}
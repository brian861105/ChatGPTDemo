using AuthServer.Infrastructure.Data.Migrations;
using Microsoft.EntityFrameworkCore;

namespace AuthServer.Infrastructure.Services;

public class AuthDbContext(DbContextOptions<AuthDbContext> options) : DbContext(options)
{
    public virtual DbSet<AuthUser> Users { get; set; }
    public virtual DbSet<AuthSession> Sessions { get; set; }
    public virtual DbSet<AuthRole> Roles { get; set; }
    public virtual DbSet<AuthUserRole> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuthUserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<AuthUser>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<AuthSession>()
            .HasIndex(s => s.Token)
            .IsUnique();

        modelBuilder.Entity<AuthSession>()
            .HasIndex(s => s.UserId);

        modelBuilder.Entity<AuthUserRole>()
            .HasIndex(ur => ur.UserId);

        modelBuilder.Entity<AuthUserRole>()
            .HasIndex(ur => ur.RoleId);

        modelBuilder.Entity<AuthUser>()
            .Property(u => u.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<AuthUser>()
            .Property(u => u.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<AuthSession>()
            .Property(s => s.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Add a trigger for updating the UpdatedAt column
        modelBuilder.HasDbFunction(() => AuthDbFunctions.UpdateTriggerFunction());
        modelBuilder.Entity<AuthUser>()
            .ToTable(tb => tb.HasTrigger("update_auth_user_updated_at"));

        modelBuilder.Entity<AuthUser>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<UserProfile>()
            .HasIndex(up => up.UserId)
            .IsUnique();

        modelBuilder.Entity<UserProfile>()
            .Property(up => up.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<UserProfile>()
            .Property(up => up.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAddOrUpdate();

        modelBuilder.Entity<UserProfile>()
            .Property(up => up.Gender)
            .HasConversion<string>();
    }
    internal static class AuthDbFunctions
    {
        [DbFunction("update_trigger_function", Schema = "public")]
        internal static DateTime UpdateTriggerFunction()
        {
            throw new NotSupportedException();
        }
    }
}


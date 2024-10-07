using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace AuthServer.Infrastructure.Data.Migrations
{
    public class AuthUser
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public required string Username { get; set; }
        [Required]
        [StringLength(255)]
        public required string PasswordHash { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<AuthSession> Sessions { get; set; }
        public virtual ICollection<AuthUserRole> UserRoles { get; set; }
    }
    public class UserProfile
    {
        [Key]
        public int ProfileId { get; set; }

        [Required]
        public int UserId { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        public DateTime? BirthDate { get; set; }

        [StringLength(20)]
        public string Gender { get; set; }

        [StringLength(20)]
        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public string Bio { get; set; }

        [StringLength(255)]
        public string AvatarUrl { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        [ForeignKey("UserId")]
        public virtual AuthUser User { get; set; }
    }
    public class AuthSession
    {
        [Key]
        public int SessionId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(255)]
        public required string Token { get; set; }

        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey("UserId")]
        public required virtual AuthUser User { get; set; }
    }

    public class AuthRole
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        [StringLength(50)]
        public required string RoleName { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        public virtual ICollection<AuthUserRole> UserRoles { get; set; }
    }

    public class AuthUserRole
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }

        [ForeignKey("UserId")]
        public required virtual AuthUser User { get; set; }

        [ForeignKey("RoleId")]
        public required virtual AuthRole Role { get; set; }
    }

    public class AuthDbContext(DbContextOptions<AuthDbContext> options) : DbContext(options)
    {
        public DbSet<AuthUser> Users { get; set; }
        public DbSet<AuthSession> Sessions { get; set; }
        public DbSet<AuthRole> Roles { get; set; }
        public DbSet<AuthUserRole> UserRoles { get; set; }

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
    }

    public static class AuthDbFunctions
    {
        [DbFunction("update_trigger_function", Schema = "public")]
        public static DateTime UpdateTriggerFunction()
        {
            throw new NotSupportedException();
        }
    }
}
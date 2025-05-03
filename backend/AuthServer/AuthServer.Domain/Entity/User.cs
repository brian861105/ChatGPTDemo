using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AuthServer.Infrastructure.Data.Models;

namespace AuthServer.Domain.Entity;

[Table("auth_users")]
public class User
{
    private User()
    {
    }

    public User(string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));

        UserId = Guid.NewGuid();
        Email = email;
        PasswordHash = passwordHash;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    [Key] [Column("user_id")] public Guid UserId { get; private set; }

    [Required]
    [StringLength(50)]
    [Column("email")]
    public string Email { get; private set; } = string.Empty;

    [Required]
    [StringLength(255)]
    [Column("password_hash")]
    public string PasswordHash { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }
    [Column("create_at")] public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    [Column("last_login_at")] public DateTime LastLoginAt { get; private set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual UserProfile? Profile { get; private set; }

    // 更新密碼
    public void UpdatePasswordHash(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("New password hash cannot be empty", nameof(newPasswordHash));

        PasswordHash = newPasswordHash;
    }

    // 領域邏輯方法
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
    public void UpdateLastLogin() => LastLoginAt = DateTime.UtcNow;
}
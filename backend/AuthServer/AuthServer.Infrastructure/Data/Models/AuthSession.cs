using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthServer.Infrastructure.Data.Models;

[Table("auth_sessions")]
public class AuthSession
{
    [Key] [Column("session_id")] public int SessionId { get; set; }

    [Required] [Column("user_id")] public int UserId { get; set; }

    [Column("token")] [StringLength(100)] public string? Token { get; set; }

    [Column("user_agent")]
    [StringLength(255)]
    public string? UserAgent { get; set; }

    [Column("ip_address")]
    [StringLength(45)]
    public string? IpAddress { get; set; }

    [Column("expires_at")] public DateTime ExpiresAt { get; set; }

    [Column("created_at")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("is_active")] public bool IsActive { get; set; } = true;

    // Navigation property
    [ForeignKey(nameof(UserId))] public virtual AuthUser User { get; set; } = null!;
}
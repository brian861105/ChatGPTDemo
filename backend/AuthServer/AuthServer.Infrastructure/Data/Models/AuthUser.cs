using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthServer.Infrastructure.Data.Models;
[Table("auth_users")]
public class AuthUser
{
    [Key] [Column("user_id")] public int UserId { get; set; }

    [Required]
    [StringLength(50)]
    [Column("email")]
    public string Email { get; set; } = null!;

    [Required]
    [StringLength(255)]
    [Column("password_hash")]
    public string PasswordHash { get; set; } = null!;

    [Column("created_at")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")] public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<AuthSession> Sessions { get; set; } = new List<AuthSession>();
    public virtual UserProfile? Profile { get; set; }
}
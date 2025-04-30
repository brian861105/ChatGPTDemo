using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthServer.Infrastructure.Data.Models;

[Table("user_profiles")]
public class UserProfile
{
    [Key] [Column("profile_id")] public int ProfileId { get; set; }

    [Required] [Column("user_id")] public int UserId { get; set; }

    [Required]
    [StringLength(50)]
    [Column("first_name")]
    public string FirstName { get; set; } = null!;

    [Required]
    [StringLength(50)]
    [Column("last_name")]
    public string LastName { get; set; } = null!;

    [Column("birth_date")] public DateTime? BirthDate { get; set; }

    [StringLength(20)] [Column("gender")] public string? Gender { get; set; }

    [StringLength(20)]
    [Column("phone_number")]
    public string? PhoneNumber { get; set; }

    [StringLength(255)]
    [Column("address")]
    public string? Address { get; set; }

    [Column("created_at")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")] public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    [ForeignKey(nameof(UserId))] public virtual AuthUser User { get; set; } = null!;
}
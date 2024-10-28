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
        public required string? PasswordHash { get; set; }
        [Required]
        [StringLength(255)]
        public required string Email { get; set; }
        [StringLength(255)]
        public string? ResetToken { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<AuthSession>? Sessions { get; set; }
        public virtual ICollection<AuthUserRole>? UserRoles { get; set; }
        public DateTime? ResetTokenExpires { get; set; }
    }
    public class UserProfile
    {
        [Key]
        public int ProfileId { get; set; }

        [Required]
        public int UserId { get; set; }

        [StringLength(50)]
        public required string FirstName { get; set; }

        [StringLength(50)]
        public required string LastName { get; set; }

        public DateTime? BirthDate { get; set; }

        [StringLength(20)]
        public required string Gender { get; set; }

        [StringLength(20)]
        public required string PhoneNumber { get; set; }

        public required string Address { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        [ForeignKey("UserId")]
        public virtual required AuthUser User { get; set; }
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

        public virtual required ICollection<AuthUserRole> UserRoles { get; set; }
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
}
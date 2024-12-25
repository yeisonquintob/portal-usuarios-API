using System;
using System.Collections.Generic;
using UserPortal.Data.Entities.Base;

namespace UserPortal.Data.Entities;

public class User : BaseEntity
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? ProfilePicture { get; set; }
    public DateTime? LastLogin { get; set; }
    public int RoleId { get; set; }
    public virtual Role? Role { get; set; }
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
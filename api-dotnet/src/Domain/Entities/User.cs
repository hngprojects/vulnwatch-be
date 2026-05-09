using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hng.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = default!;
    public string? PasswordHash { get; set; }      // null for Google OAuth
    public string? GoogleId { get; set; }
    public bool IsEmailVerified { get; set; } = false;
    public string? VerificationToken { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

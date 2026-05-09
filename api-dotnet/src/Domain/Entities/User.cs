using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class User : IdentityUser<Guid>
{
    public string? GoogleId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public static User Create(string email) => new()
    {
        Id = Guid.NewGuid(),
        Email = email,
        UserName = email,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    public void Activate()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}

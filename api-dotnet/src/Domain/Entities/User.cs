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

    public static User CreateFromGoogle(string email, string googleId) => new()
    {
        Id = Guid.NewGuid(),
        Email = email,
        UserName = email,
        EmailConfirmed = true,
        GoogleId = googleId,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    public bool LinkGoogleAccount(string googleId)
    {
        if (string.Equals(GoogleId, googleId, StringComparison.Ordinal))
            return false;

        GoogleId = googleId;
        Touch();
        return true;
    }

    public bool UpdateEmailAddress(string email)
    {
        if (string.Equals(Email, email, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(UserName, email, StringComparison.OrdinalIgnoreCase))
            return false;

        Email = email;
        UserName = email;
        Touch();
        return true;
    }

    public bool ConfirmEmail()
    {
        if (EmailConfirmed)
            return false;

        EmailConfirmed = true;
        Touch();
        return true;
    }

    private void Touch() => UpdatedAt = DateTime.UtcNow;
}

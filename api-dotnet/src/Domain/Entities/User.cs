using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class ApplicationUser: IdentityUser
{}

public class User : EntityBase
{
    public string Email { get; private set; } = default!;
    public string? PasswordHash { get; private set; }
    public string? GoogleId { get; private set; }
    public bool IsEmailVerified { get; private set; }
    public string? VerificationToken { get; private set; }

    private User() { }

    public static User Create(string email, string? passwordHash = null, string? googleId = null, string? verificationToken = null)
        => new()
        {
            Email = email,
            PasswordHash = passwordHash,
            GoogleId = googleId,
            VerificationToken = verificationToken,
        };

    public void VerifyEmail()
    {
        IsEmailVerified = true;
        VerificationToken = null;
        Touch();
    }
}

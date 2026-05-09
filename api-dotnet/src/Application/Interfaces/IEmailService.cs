

namespace Application.Interfaces;
public interface IEmailService
{
    // Task SendEmail(List<string> recipientEmail, string subject, string templatePath, Dictionary<string, string> replacements, string activity);

    Task SendPasswordReset(string email, string token);
}
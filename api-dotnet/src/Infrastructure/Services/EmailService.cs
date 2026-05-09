using System.Net;
using System.Net.Mail;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config) => _config = config;

    public async Task SendAsync(string to, string subject, string body)
    {
        var host = _config["SmtpCredentials:Host"]!;
        var port = int.Parse(_config["SmtpCredentials:Port"]!);
        var username = _config["SmtpCredentials:Username"]!;
        var password = _config["SmtpCredentials:Password"]!;

        using var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(username, password),
            EnableSsl = true
        };

        await client.SendMailAsync(new MailMessage(username, to, subject, body));
    }
}

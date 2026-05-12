using Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class NotificationDispatcherStub : INotificationDispatcher
{
    private readonly ILogger<NotificationDispatcherStub> _logger;

    public NotificationDispatcherStub(ILogger<NotificationDispatcherStub> logger)
    {
        _logger = logger;
    }

    public async Task SendScanCompleteAsync(Guid userId, Guid scanId, int securityScore, CancellationToken ct)
    {
        // Stub implementation — logs the notification instead of sending
        _logger.LogInformation(
            "Notification stub: Scan complete notification would be sent to user '{UserId}' for scan '{ScanId}' with security score {SecurityScore}.",
            userId, scanId, securityScore);

        // Simulate async work
        await Task.CompletedTask;
    }
}

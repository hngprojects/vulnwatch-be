namespace Application.Interfaces.Services;

public interface INotificationDispatcher
{
    Task SendScanCompleteAsync(Guid userId, Guid scanId, int securityScore, CancellationToken ct);
}

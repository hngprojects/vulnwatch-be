using Application.Interfaces;
using Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Application.Features.Scans.ProcessScanResult;

public interface IProcessScanResultHandler
{
    Task HandleAsync(ScanResultMessage message, CancellationToken ct);
}

public class ProcessScanResultHandler : IProcessScanResultHandler
{
    private readonly IScanRepository _scanRepository;
    private readonly INotificationDispatcher _notificationDispatcher;
    private readonly ILogger<ProcessScanResultHandler> _logger;

    public ProcessScanResultHandler(IScanRepository scanRepository, INotificationDispatcher notificationDispatcher, ILogger<ProcessScanResultHandler> logger)
    {
        _scanRepository = scanRepository;
        _notificationDispatcher = notificationDispatcher;
        _logger = logger;
    }

    public async Task HandleAsync(ScanResultMessage message, CancellationToken ct)
    {
        // Load scan by scanId
        var scan = await _scanRepository.GetByIdAsync(message.ScanId, ct);

        if (scan is null)
        {
            _logger.LogWarning("Scan with ID '{ScanId}' not found. Ignoring result message.", message.ScanId);
            return;
        }

        try
        {
            // Update scan with result data
            scan.Complete(message.SecurityScore);

            // Persist changes
            await _scanRepository.UpdateAsync(scan, ct);

            _logger.LogInformation("Scan '{ScanId}' updated with status '{Status}' and security score {SecurityScore}.", 
                message.ScanId, message.Status, message.SecurityScore);

            // Send notification
            await _notificationDispatcher.SendScanCompleteAsync(scan.UserId, scan.Id, message.SecurityScore, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing scan result for scan ID '{ScanId}'.", message.ScanId);
            throw;
        }
    }
}

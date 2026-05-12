using Application.Features.Scans.ProcessScanResult;
using Application.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.BackgroundServices;

public class ScanResultListenerService : BackgroundService
{
    private readonly IRedisConsumer _redisConsumer;
    private readonly IProcessScanResultHandler _processScanResultHandler;
    private readonly ILogger<ScanResultListenerService> _logger;

    public ScanResultListenerService(IRedisConsumer redisConsumer, IProcessScanResultHandler processScanResultHandler, ILogger<ScanResultListenerService> logger)
    {
        _redisConsumer = redisConsumer;
        _processScanResultHandler = processScanResultHandler;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Scan result listener service started.");

        try
        {
            await _redisConsumer.StartListeningAsync(
                "scan-results",
                async (messageJson) => await OnMessageReceivedAsync(messageJson, stoppingToken),
                stoppingToken
            );
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Scan result listener service cancelled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fatal error in scan result listener service.");
            throw;
        }
    }

    private async Task OnMessageReceivedAsync(string messageJson, CancellationToken ct)
    {
        try
        {
            _logger.LogDebug("Received message from scan-results channel: {Message}", messageJson);

            // Deserialise JSON into ScanResultMessage
            var message = JsonSerializer.Deserialize<ScanResultMessage>(messageJson);

            if (message is null)
            {
                _logger.LogWarning("Failed to deserialise message: {MessageJson}", messageJson);
                return;
            }

            // Process the scan result
            await _processScanResultHandler.HandleAsync(message, ct);

            _logger.LogInformation("Processed scan result for scan ID '{ScanId}'.", message.ScanId);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse message as JSON: {Message}", messageJson);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message from scan-results channel.");
            // Continue listening despite the error
        }
    }
}

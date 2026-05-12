using Application.Interfaces;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Infrastructure.Redis;

public class RedisConsumer : IRedisConsumer
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisConsumer> _logger;

    public RedisConsumer(IConnectionMultiplexer redis, ILogger<RedisConsumer> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task StartListeningAsync(string channel, Func<string, Task> onMessageReceived, CancellationToken ct)
    {
        try
        {
            var db = _redis.GetDatabase();

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    // BLPOP with timeout of 1 second allows periodic cancellation token checks
                    var result = await db.ListLeftPopAsync(channel);

                    if (!result.IsNull)
                    {
                        var message = result.ToString();
                        await onMessageReceived(message);
                    }
                    else
                    {
                        // No message available, wait a bit to avoid tight loop
                        await Task.Delay(TimeSpan.FromMilliseconds(100), ct);
                    }
                }
                catch (RedisConnectionException ex)
                {
                    _logger.LogError(ex, "Redis connection error while listening on channel '{Channel}'. Retrying...", channel);
                    // Wait a bit before retrying to avoid tight loop
                    await Task.Delay(TimeSpan.FromSeconds(1), ct);
                }
                catch (OperationCanceledException)
                {
                    // Expected when cancellation is requested
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error while listening on channel '{Channel}'. Continuing...", channel);
                    // Continue listening despite the error
                }
            }

            _logger.LogInformation("Redis consumer stopped listening on channel '{Channel}'.", channel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fatal error in Redis consumer for channel '{Channel}'.", channel);
            throw;
        }
    }
}

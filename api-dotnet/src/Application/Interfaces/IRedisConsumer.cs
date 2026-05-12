namespace Application.Interfaces;

public interface IRedisConsumer
{
    Task StartListeningAsync(string channel, Func<string, Task> onMessageReceived, CancellationToken ct);
}

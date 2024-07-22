using Notifier.Lambda.Domain.Channels;
using Notifier.Lambda.Domain.Events;
using Polly;
using Polly.Timeout;
using Serilog;

namespace Notifier.Lambda.Application.Services;

/// <inheritdoc />
public class ChannelService(IEnumerable<IChannel> channels, ILogger logger) : IChannelService
{
    // can be configurable
    private readonly IAsyncPolicy _timeoutPolicy = Policy
        .TimeoutAsync(TimeSpan.FromSeconds(10), TimeoutStrategy.Pessimistic);
    //add throw if null

    public async Task SendAsync(NotificationEvent notificationEvent, CancellationToken cancellationToken)
    {
        var channel = channels.FirstOrDefault(c => c.IsEnabled && c.Name.StartsWith(notificationEvent.EventType, StringComparison.OrdinalIgnoreCase));

        if (channel == null)
        {
            throw new Exception($"Channel for event type {notificationEvent.EventType} is not enabled or does not exist.");
        }

        try
        {
            await _timeoutPolicy.ExecuteAsync(async ct =>
            {
                await channel.SendAsync(notificationEvent);
            }, cancellationToken);
        }
        catch (TimeoutRejectedException)
        {
            logger.Error($"Timeout occurred while sending message {notificationEvent.Id}.");
        }
    }
}
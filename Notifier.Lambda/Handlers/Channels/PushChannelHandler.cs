using Notifier.Lambda.Application.Options.Channel;
using Notifier.Lambda.Application.Services;
using Notifier.Lambda.Domain.Channels;
using Notifier.Lambda.Domain.Events;

namespace Notifier.Lambda.Handlers.Channels;

public class PushChannelHandler(ChannelOptions channelOptions, IProviderService providerService) : IChannel
{
    public bool IsEnabled => channelOptions.Push.Enabled; // This can be configurable
    public string Name => channelOptions.Push.Name;

    public async Task SendAsync(NotificationEvent notificationEvent)
    {
        var pushEvent = notificationEvent as PushEvent;
        if (pushEvent == null) throw new InvalidCastException("NotificationEvent is not a PushEvent");

        // Logic to send push notification using configured providers
        await providerService.HandleSendAsync(notificationEvent);
    }
}
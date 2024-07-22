using Notifier.Lambda.Application.Services;
using Notifier.Lambda.Domain.Channels;
using Notifier.Lambda.Domain.Events;
using ChannelOptions = Notifier.Lambda.Application.Options.Channel.ChannelOptions;

namespace Notifier.Lambda.Handlers.Channels;

public class EmailChannelHandler(ChannelOptions channelOptions, IProviderService providerService) : IChannel
{
    public bool IsEnabled => channelOptions.Email.Enabled;
    public string Name => channelOptions.Email.Name;

    public async Task SendAsync(NotificationEvent notificationEvent)
    {
        var emailEvent = notificationEvent as EmailEvent;
        if (emailEvent == null) throw new InvalidCastException("NotificationEvent is not an EmailEvent");

        // Logic to send email using configured providers
        await providerService.HandleSendAsync(notificationEvent);
    }
}
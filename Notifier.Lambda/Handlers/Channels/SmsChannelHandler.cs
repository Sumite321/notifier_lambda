using Notifier.Lambda.Application.Options.Channel;
using Notifier.Lambda.Application.Services;
using Notifier.Lambda.Domain.Channels;
using Notifier.Lambda.Domain.Events;

namespace Notifier.Lambda.Handlers.Channels;

public class SmsChannelHandler(ChannelOptions channelOptions, IProviderService providerService) : IChannel
{
    public bool IsEnabled => channelOptions.Sms.Enabled; 
    public string Name => channelOptions.Sms.Name;

    public async Task SendAsync(NotificationEvent notificationEvent)
    {
        if (notificationEvent is not SMSEvent smsEvent) throw new InvalidCastException("NotificationEvent is not an SMSEvent");

        // Logic to send SMS using configured providers
        await providerService.HandleSendAsync(notificationEvent);
    }
}
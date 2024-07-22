using Notifier.Lambda.Domain.Events;

namespace Notifier.Lambda.Domain.Channels;

public interface IChannel
{
    bool IsEnabled { get; }
    string Name { get; }
    Task SendAsync(NotificationEvent notificationEvent);
}
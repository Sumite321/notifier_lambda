namespace Notifier.Lambda.Domain.Events;

public abstract class NotificationEvent
{
    public string Id { get; set; }
    public string EventType { get; set; }
}
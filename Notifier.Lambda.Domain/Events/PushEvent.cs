namespace Notifier.Lambda.Domain.Events;

public class PushEvent : NotificationEvent
{
    public string? DeviceToken { get; set; }
    public string? Message { get; set; }
}
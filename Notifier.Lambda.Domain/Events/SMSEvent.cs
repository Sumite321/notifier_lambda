namespace Notifier.Lambda.Domain.Events;

public class SMSEvent : NotificationEvent
{
    public string? PhoneNumber { get; set; }
    public string? Message { get; set; }
}
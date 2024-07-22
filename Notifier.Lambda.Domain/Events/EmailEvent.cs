namespace Notifier.Lambda.Domain.Events;

public class EmailEvent : NotificationEvent
{
    public string EmailAddress { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}
using Notifier.Lambda.Domain.Events;

namespace Notifier.Lambda.Application.Services;

public interface IProviderService
{
    Task HandleSendAsync(NotificationEvent notification);
}
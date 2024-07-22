using Notifier.Lambda.Domain.Events;

namespace Notifier.Lambda.Application.Services
{
    /// <summary>
    /// Interface for a service responsible for sending notifications through various channels.
    /// </summary>
    public interface IChannelService
    {
        /// <summary>
        /// Sends a notification event through the appropriate channel.
        /// </summary>
        /// <param name="notificationEvent">The notification event to be sent.</param>
        /// <param name="cancellationToken">The cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SendAsync(NotificationEvent notificationEvent, CancellationToken cancellationToken);
    }
}
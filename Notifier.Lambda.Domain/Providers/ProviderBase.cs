using Notifier.Lambda.Domain.Events;

namespace Notifier.Lambda.Domain.Providers
{
    /// <summary>
    /// Represents a base class for notification providers.
    /// This class defines the common contract that all concrete notification providers must implement.
    /// </summary>
    public abstract class ProviderBase
    {
        /// <summary>
        /// Gets a value indicating whether this provider is enabled.
        /// Providers can be enabled or disabled based on configuration or operational needs.
        /// </summary>
        public abstract bool IsEnabled { get; }

        /// <summary>
        /// Gets the priority of the provider.
        /// This can be used to determine the order of provider selection in case of multiple providers.
        /// Higher values typically indicate higher priority.
        /// </summary>
        public abstract int Priority { get; }

        /// <summary>
        /// Gets the name of the provider.
        /// This is a unique identifier for the provider, useful for logging and configuration purposes.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Sends a notification asynchronously using the provider.
        /// Implementations must handle the specifics of sending notifications via their respective channels.
        /// </summary>
        /// <param name="notification">The notification event to be sent.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public abstract Task SendAsync(NotificationEvent notification);

        /// <summary>
        /// Checks the health of the provider.
        /// This method can be used to verify whether the provider is operational and capable of handling requests.
        /// Implementations should perform necessary health checks and return true if the provider is healthy.
        /// </summary>
        /// <returns>True if the provider is healthy; otherwise, false.</returns>
        public abstract bool CheckHealth();
    }
}

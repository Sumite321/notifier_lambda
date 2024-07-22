using Notifier.Lambda.Application.Options.Provider;
using Notifier.Lambda.Domain.Events;
using Notifier.Lambda.Domain.Providers;

namespace Notifier.Lambda.Handlers.Providers;

/// <inheritdoc />
public class SnsProviderHandler(ProviderOptions providerOptions) : ProviderBase
{
    public override bool IsEnabled => providerOptions.Sns.Enabled;  // Read from configuration
    public override int Priority => providerOptions.Sns.Priority; // Read from configuration
    public override string Name => providerOptions.Sns.Name; // Read from configuration

    public override Task SendAsync(NotificationEvent notification)
    {
        // Implement SNS API call

        return Task.CompletedTask;
    }

    public override bool CheckHealth()
    {
        // Implement health check logic for SNS
        return true;
    }
}
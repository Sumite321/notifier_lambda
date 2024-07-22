using Notifier.Lambda.Application.Options.Provider;
using Notifier.Lambda.Domain.Events;
using Notifier.Lambda.Domain.Providers;

namespace Notifier.Lambda.Handlers.Providers;

public class VonageProviderHandler(ProviderOptions providerOptions) : ProviderBase
{
    public override bool IsEnabled => providerOptions.Vonage.Enabled;  // Read from configuration
    public override int Priority => providerOptions.Vonage.Priority; // Read from configuration
    public override string Name => providerOptions.Vonage.Name; // Read from configuration

    public override Task SendAsync(NotificationEvent notification)
    {
        // Implement Vonage API call#
        return Task.CompletedTask;
    }

    public override bool CheckHealth()
    {
        // Implement health check logic for Vonage
        return true;
    }
}
using Notifier.Lambda.Application.Options.Provider;
using Notifier.Lambda.Domain.Events;
using Notifier.Lambda.Domain.Providers;

namespace Notifier.Lambda.Handlers.Providers;

public class TwilioProviderHandler(ProviderOptions providerOptions) : ProviderBase
{
    public override bool IsEnabled => providerOptions.Twilio.Enabled;  // Read from configuration
    public override int Priority => providerOptions.Twilio.Priority; // Read from configuration
    public override string Name => providerOptions.Twilio.Name; // Read from configuration

    public override Task SendAsync(NotificationEvent notification)
    {
        // Implement Twilio API call
        return Task.CompletedTask;
    }

    public override bool CheckHealth()
    {
        // Implement health check logic for Twilio
        return true;
    }
}
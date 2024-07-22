namespace Notifier.Lambda.Application.Options.Provider
{
    /// <summary>
    /// Represents configuration settings for different notification providers.
    /// These settings are bound from configuration files or environment variables.
    /// </summary>
    public class ProviderOptions
    {
        /// <summary>
        /// Configuration options for the SNS (Simple Notification Service) provider.
        /// </summary>
        public SnsProviderOptions Sns { get; set; }

        /// <summary>
        /// Configuration options for the Twilio provider.
        /// </summary>
        public TwilioProviderOptions Twilio { get; set; }

        /// <summary>
        /// Configuration options for the Vonage provider.
        /// </summary>
        public VonageProviderOptions Vonage { get; set; }
    }
}
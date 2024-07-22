namespace Notifier.Lambda.Application.Options.Channel
{
    /// <summary>
    /// Represents configuration settings for different notification channels.
    /// These settings are bound from configuration files or environment variables.
    /// </summary>
    public class ChannelOptions
    {
        /// <summary>
        /// Configuration options for the SMS (Short Message Service) channel.
        /// </summary>
        public SMSOptions? Sms { get; set; }

        /// <summary>
        /// Configuration options for the Push notification channel.
        /// </summary>
        public PushOptions? Push { get; set; }

        /// <summary>
        /// Configuration options for the Email channel.
        /// </summary>
        public EmailOptions? Email { get; set; }
    }
}
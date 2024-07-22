using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Notifier.Lambda.Domain.Events;
using Notifier.Lambda.Domain.Providers;
using Polly;

namespace Notifier.Lambda.Application.Services;

/// <inheritdoc />
public class ProviderService : IProviderService
{
    private readonly IEnumerable<ProviderBase> _providers;
    private readonly Serilog.ILogger _logger;
    private readonly IAmazonSQS _amazonSqs;
    private const string DlqQueueUrl = "notifier_dql"; // can be configurable

    public ProviderService(IEnumerable<ProviderBase> providers, Serilog.ILogger logger, IAmazonSQS sqsClient)
    {
        _providers = providers
            .Where(p => p.IsEnabled)
            .OrderBy(p => p.Priority);
        _logger = logger;
        _amazonSqs = sqsClient;
    }

    public async Task HandleSendAsync(NotificationEvent notificationEvent)
    {
        foreach (var provider in _providers)
        {
            try
            {
                // this policy can be added as global and configurable
                await Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(2, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
                    .ExecuteAsync(async () =>
                    {
                        await provider.SendAsync(notificationEvent);
                    });

                _logger.Information($"Message sent successfully via {provider.Name}");
                return; // Exit if sending is successful
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Failed to send message via {provider.Name}. Trying next provider.");
            }
        }

        // If all providers fail, log an error indicating that all attempts failed
        _logger.Error("All providers failed to send the message.");
        await PushToDlq(notificationEvent);
    }
        
    private async Task PushToDlq(NotificationEvent notificationEvent)
    {
        var sendMessageRequest = new SendMessageRequest
        {
            QueueUrl = DlqQueueUrl,
            MessageBody = JsonSerializer.Serialize(notificationEvent)
        };
        await _amazonSqs.SendMessageAsync(sendMessageRequest);
    }
}
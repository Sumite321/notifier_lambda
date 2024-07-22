using Amazon.Lambda.Annotations;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.SQS;
using Amazon.SQS.Model;
using Notifier.Lambda.Application.Services;
using Serilog;
using System.Text.Json;
using Notifier.Lambda.Domain.Events;
using Notifier.Lambda.Infrastructure.Redis;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Notifier.Lambda;

public class Function
{
    private readonly IAmazonSQS _sqsClient;
    private readonly ILogger _logger;
    private readonly IChannelService _channelService;
    private readonly IRedisService _redisService;
    private const string DlqQueueUrl = "notifier_dql"; // can be configurable

    public Function(IAmazonSQS amazonSqs, ILogger logger, IChannelService channelService, IRedisService redisService)
    {
        _sqsClient = amazonSqs;
        _logger = logger;
        _channelService = channelService;
        _redisService = redisService;
    }
    /// <summary>
    /// Starter function that takes a sqs event and processes it. Adds to DQL if any issues
    /// </summary>
    /// <param name="sqsEvent">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task FunctionHandler(
        SQSEvent sqsEvent,
        ILambdaContext context,
        CancellationToken cancellationToken)
    {
        foreach (var message in sqsEvent.Records)
        {
            var notificationEvent = JsonSerializer.Deserialize<NotificationEvent>(message.Body);

            if (notificationEvent == null)
            {
                _logger.Error("Received null notification event.");
                return;
            }

            if (await _redisService.HasBeenProcessedAsync(notificationEvent.Id))
            {
                _logger.Information($"Notification with ID {notificationEvent.Id} has already been processed.");
                return;
            }

            try
            {
                _logger.Information($"Processing message ID: {message.MessageId}");
                await _channelService.SendAsync(notificationEvent, cancellationToken);
                // avoid double processing
                await _redisService.MarkAsProcessedAsync(notificationEvent.Id);
                _logger.Information($"Notification with ID {notificationEvent.Id} processed successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to send notification: {ex.Message}");
                await PushToDlq(notificationEvent);
            }

        }
    }

    private async Task PushToDlq(NotificationEvent notificationEvent)
    {
        var sendMessageRequest = new SendMessageRequest
        {
            QueueUrl = DlqQueueUrl,
            MessageBody = JsonSerializer.Serialize(notificationEvent)
        };
        await _sqsClient.SendMessageAsync(sendMessageRequest);
    }
}
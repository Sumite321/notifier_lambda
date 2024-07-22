using Amazon.SQS;
using Amazon.SQS.Model;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using Notifier.Lambda.Application.Services;
using Notifier.Lambda.Domain.Providers;
using Notifier.Lambda.Domain.Events;
using Serilog;

namespace Notifier.Lambda.Tests.Application.Services;

public class ProviderServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<ILogger> _mockLogger;
    private readonly Mock<IAmazonSQS> _mockDLQService; //it can be a dlqservice in the future
    private readonly List<Mock<ProviderBase>> _mockProviders;
    private readonly ProviderService _providerService;

    public ProviderServiceTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mockLogger = _fixture.Freeze<Mock<ILogger>>();
        _mockDLQService = _fixture.Freeze<Mock<IAmazonSQS>>();
        _mockProviders = _fixture.CreateMany<Mock<ProviderBase>>(3).ToList();

        // Setup mock providers with different priorities
        _mockProviders[0].Setup(p => p.Priority).Returns(1);
        _mockProviders[1].Setup(p => p.Priority).Returns(2);
        _mockProviders[2].Setup(p => p.Priority).Returns(3);        
        
        _providerService = new ProviderService(_mockProviders.Select(p => p.Object), _mockLogger.Object, _mockDLQService.Object);
    }

    [Fact]
    public async Task SendAsync_ShouldSendViaFirstProvider_WhenFirstProviderSucceeds()
    {
        // Arrange
        var notificationEvent = _fixture.Create<NotificationEvent>();
        _mockProviders[0].Setup(p => p.SendAsync(notificationEvent)).Returns(Task.CompletedTask);
        _mockProviders[0].Setup(p => p.IsEnabled).Returns(true);

        // Act
        await _providerService.HandleSendAsync(notificationEvent);

        // Assert
        _mockProviders[0].Verify(p => p.SendAsync(notificationEvent), Times.Once);
        _mockProviders[1].Verify(p => p.SendAsync(notificationEvent), Times.Never);
        _mockProviders[2].Verify(p => p.SendAsync(notificationEvent), Times.Never);
    }

    [Fact]
    public async Task SendAsync_ShouldRetryAndFailoverToNextProvider_WhenFirstProviderFails()
    {
        // Arrange
        var notificationEvent = _fixture.Create<NotificationEvent>();
        _mockProviders[0].Setup(p => p.SendAsync(notificationEvent)).ThrowsAsync(new Exception());
        _mockProviders[1].Setup(p => p.SendAsync(notificationEvent)).Returns(Task.CompletedTask);
        _mockProviders[0].Setup(p => p.IsEnabled).Returns(true);
        _mockProviders[1].Setup(p => p.IsEnabled).Returns(true);

        // Act
        await _providerService.HandleSendAsync(notificationEvent);

        // Assert
        _mockProviders[0].Verify(p => p.SendAsync(notificationEvent), Times.Exactly(3)); // Retry 3 times
        _mockProviders[1].Verify(p => p.SendAsync(notificationEvent), Times.Once);
        _mockProviders[2].Verify(p => p.SendAsync(notificationEvent), Times.Never);
    }

    [Fact]
    public async Task SendAsync_ShouldAddToDLQ_WhenAllProvidersFail()
    {
        _mockProviders[0].Setup(p => p.IsEnabled).Returns(true);

        // Arrange
        var notificationEvent = _fixture.Create<NotificationEvent>();
        _mockProviders.ForEach(p => p.Setup(provider => provider.SendAsync(notificationEvent)).ThrowsAsync(new Exception()));

        // Act
        await _providerService.HandleSendAsync(notificationEvent);

        // Assert
        _mockProviders.First().Verify(provider => provider.SendAsync(notificationEvent), Times.Exactly(3)); // Each retries 5 times
        _mockDLQService.Verify(dlq => dlq.SendMessageAsync(It.IsAny<SendMessageRequest>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task SendAsync_ShouldLogSuccess_WhenMessageSentSuccessfully()
    {
        // Arrange
        var notificationEvent = _fixture.Create<NotificationEvent>();
        _mockProviders[0].Setup(p => p.SendAsync(notificationEvent)).Returns(Task.CompletedTask);
        _mockProviders[0].Setup(p => p.IsEnabled).Returns(true);

        // Act
        await _providerService.HandleSendAsync(notificationEvent);

        // Assert
        _mockLogger.Verify(logger => logger.Information(It.Is<string>(msg => msg.Contains("Message sent successfully via"))), Times.Once);
    }

    [Fact]
    public async Task SendAsync_ShouldLogError_WhenProviderFails()
    {
        // Arrange
        var notificationEvent = _fixture.Create<NotificationEvent>();
        _mockProviders[0].Setup(p => p.SendAsync(notificationEvent)).ThrowsAsync(new Exception());
        _mockProviders[0].Setup(p => p.IsEnabled).Returns(true);

        // Act
        await _providerService.HandleSendAsync(notificationEvent);

        // Assert
        _mockLogger.Verify(logger => logger.Error(It.IsAny<Exception>(), It.Is<string>(msg => msg.Contains("Failed to send message via"))), Times.AtLeastOnce);
    }

    [Fact]
    public async Task SendAsync_ShouldLogError_WhenAllProvidersFail()
    {
        // Arrange
        var notificationEvent = _fixture.Create<NotificationEvent>();
        _mockProviders.ForEach(p => p.Setup(provider => provider.SendAsync(notificationEvent)).ThrowsAsync(new Exception()));

        // Act
        await _providerService.HandleSendAsync(notificationEvent);

        // Assert
        _mockLogger.Verify(logger => logger.Error(It.Is<string>(msg => msg.Contains("All providers failed to send the message"))), Times.Once);
    }
}
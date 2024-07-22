using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using Notifier.Lambda.Application.Services;
using Notifier.Lambda.Domain.Channels;
using Notifier.Lambda.Domain.Events;
using ILogger = Serilog.ILogger;

namespace Notifier.Lambda.Tests.Application.Services
{
    public class ChannelServiceTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<ILogger> _mockLogger;
        private readonly List<Mock<IChannel>> _mockChannels;
        private readonly ChannelService _channelService;

        public ChannelServiceTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mockLogger = _fixture.Freeze<Mock<ILogger>>();
            _mockChannels = _fixture.CreateMany<Mock<IChannel>>(3).ToList();

            // Setup mock channels with IsEnabled
            _mockChannels.ForEach(c => c.SetupGet(channel => channel.IsEnabled).Returns(true));

            _channelService = new ChannelService(_mockChannels.Select(c => c.Object), _mockLogger.Object);
        }

        [Fact]
        public async Task SendAsync_ShouldSendViaCorrectChannel_WhenChannelIsEnabled()
        {
            // Arrange
            var notificationEvent = _fixture.Build<EmailEvent>()
                                             .With(ne => ne.EventType, "Email")
                                             .Create();
            var emailChannel = _mockChannels.First();
            emailChannel.Setup(c => c.Name).Returns("EmailChannelHandler");
            emailChannel.Setup(c => c.SendAsync(notificationEvent)).Returns(Task.CompletedTask);

            // Act
            await _channelService.SendAsync(notificationEvent, CancellationToken.None);

            // Assert
            emailChannel.Verify(c => c.SendAsync(notificationEvent), Times.Once);
        }

        [Fact]
        public void SendAsync_ShouldThrowException_WhenChannelIsNotEnabled()
        {
            // Arrange
            var notificationEvent = _fixture.Build<SMSEvent>()
                                             .With(ne => ne.EventType, "Sms")
                                             .Create();
            _mockChannels.ForEach(c => c.SetupGet(channel => channel.IsEnabled).Returns(false));

            // Act
            Func<Task> act = async () => await _channelService.SendAsync(notificationEvent, CancellationToken.None);

            // Assert
            act.Should().ThrowAsync<Exception>().WithMessage("Channel for event type Sms is not enabled or does not exist.");
        }

        [Fact]
        public void SendAsync_ShouldThrowException_WhenChannelDoesNotExist()
        {
            // Arrange
            var notificationEvent = _fixture.Build<PushEvent>()
                                             .With(ne => ne.EventType, "Push")
                                             .Create();

            // Act
            Func<Task> act = async () => await _channelService.SendAsync(notificationEvent, CancellationToken.None);

            // Assert
            act.Should().ThrowAsync<Exception>().WithMessage("Channel for event type Push is not enabled or does not exist.");
        }

        [Fact]
        public async Task SendAsync_ShouldLogError_WhenTimeoutOccurs()
        {
            // Arrange
            var notificationEvent = _fixture.Build<EmailEvent>()
                                             .With(ne => ne.EventType, "Email")
                                             .Create();
            var emailChannel = _mockChannels.First();
            emailChannel.Setup(c => c.Name).Returns("EmailChannelHandler");
            emailChannel.Setup(c => c.SendAsync(notificationEvent)).Returns(Task.Delay(TimeSpan.FromSeconds(15)));

            // Act
            await _channelService.SendAsync(notificationEvent, CancellationToken.None);

            // Assert
            _mockLogger.Verify(logger => logger.Error(It.Is<string>(msg => msg.Contains($"Timeout occurred while sending message {notificationEvent.Id}"))), Times.Once);
        }
    }
}

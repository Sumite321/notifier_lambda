using AutoFixture;
using FluentAssertions;
using Moq;
using Notifier.Lambda.Application.Options.Channel;
using Notifier.Lambda.Application.Services;
using Notifier.Lambda.Domain.Events;
using Notifier.Lambda.Domain.Providers;
using Notifier.Lambda.Handlers.Channels;
using Xunit;

namespace Notifier.Lambda.Domain.Tests.Channel;

public class EmailChannelHandlerTests
{
    private readonly IFixture _fixture;
    private readonly EmailChannelHandler _emailChannelHandler;
    private readonly ChannelOptions _channelOptions;
    private readonly Mock<ProviderBase> _mockTwilioProvider;
    private readonly Mock<ProviderBase> _mockSnsProvider;
    private readonly Mock<IProviderService> _mockProviderHandler;

    public EmailChannelHandlerTests()
    {
        _fixture = new Fixture();
        _channelOptions = new ChannelOptions() { Email = new EmailOptions() { Enabled = true } };
        _mockTwilioProvider = new Mock<ProviderBase>();
        _mockSnsProvider = new Mock<ProviderBase>();
        _mockProviderHandler = new Mock<IProviderService>();

        _mockTwilioProvider.Setup(p => p.IsEnabled).Returns(true);
        _mockTwilioProvider.Setup(p => p.Priority).Returns(1);

        _mockSnsProvider.Setup(p => p.IsEnabled).Returns(true);
        _mockSnsProvider.Setup(p => p.Priority).Returns(2);
        

        _emailChannelHandler = new EmailChannelHandler(_channelOptions, _mockProviderHandler.Object);
    }

    [Fact]
    public void Send_ShouldSendEmailNotification()
    {
        var notification = _fixture.Create<EmailEvent>();
        notification.EmailAddress = "Email@abc.com";

        _emailChannelHandler.Invoking(channel => channel.SendAsync(notification))
            .Should().NotThrowAsync();
    }

    [Fact]
    public void IsEnabled_ShouldReturnTrue()
    {
        _emailChannelHandler.IsEnabled.Should().BeTrue();
    }
}
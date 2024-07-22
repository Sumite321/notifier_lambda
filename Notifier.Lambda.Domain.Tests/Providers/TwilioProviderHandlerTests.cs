using AutoFixture;
using FluentAssertions;
using Notifier.Lambda.Application.Options.Provider;
using Notifier.Lambda.Domain.Events;
using Notifier.Lambda.Handlers.Providers;
using Xunit;

namespace Notifier.Lambda.Domain.Tests.Providers;

public class TwilioProviderHandlerTests
{
    private readonly IFixture _fixture;
    private readonly TwilioProviderHandler _twilioProviderHandler;

    public TwilioProviderHandlerTests()
    {
        _fixture = new Fixture();
        var providerOptions = new ProviderOptions()
            { Twilio = new TwilioProviderOptions() { Enabled = true, Priority = 1 } };
        _twilioProviderHandler = new TwilioProviderHandler(providerOptions);
    }

    [Fact]
    public void Send_ShouldSendNotificationViaTwilio()
    {
        var notification = _fixture.Create<PushEvent>();

        _twilioProviderHandler.Invoking(provider => provider.SendAsync(notification))
            .Should().NotThrowAsync();
    }

    [Fact]
    public void CheckHealth_ShouldReturnTrue()
    {
        _twilioProviderHandler.CheckHealth().Should().BeTrue();
    }

    [Fact]
    public void IsEnabled_ShouldReturnTrue()
    {
        _twilioProviderHandler.IsEnabled.Should().BeTrue();
    }

    [Fact]
    public void Priority_ShouldReturnOne()
    {
        _twilioProviderHandler.Priority.Should().Be(1);
    }
}
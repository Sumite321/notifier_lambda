using AutoFixture;
using FluentAssertions;
using Moq;
using Notifier.Lambda.Application.HealthChecks;
using Notifier.Lambda.Domain.Providers;

namespace Notifier.Lambda.Tests.Application.HealthChecks;

public class HealthCheckServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<ProviderBase> _mockTwilioProvider;
    private readonly Mock<ProviderBase> _mockSnsProvider;
    private readonly HealthCheckService _healthCheckService;

    public HealthCheckServiceTests()
    {
        _fixture = new Fixture();
        _mockTwilioProvider = new Mock<ProviderBase>();
        _mockSnsProvider = new Mock<ProviderBase>();

        _mockTwilioProvider.Setup(p => p.CheckHealth()).Returns(true);
        _mockTwilioProvider.Setup(p => p.Name).Returns("TwilioProviderHandler");
        _mockSnsProvider.Setup(p => p.CheckHealth()).Returns(true);
        _mockSnsProvider.Setup(p => p.Name).Returns("SnsProvider");

        var providers = new List<ProviderBase> { _mockTwilioProvider.Object, _mockSnsProvider.Object };

        _healthCheckService = new HealthCheckService(providers);
    }

    [Fact]
    public void IsHealthy_ShouldReturnTrueIfAllProvidersAreHealthy()
    {
        var result = _healthCheckService.IsHealthy();

        result.Should().BeTrue();
    }

    [Fact]
    public void GetHealthStatus_ShouldReturnCorrectHealthStatus()
    {
        var result = _healthCheckService.GetHealthStatus();

        result.Should().Contain(new KeyValuePair<string, bool>(_mockTwilioProvider.Object.Name, true));
        result.Should().Contain(new KeyValuePair<string, bool>(_mockSnsProvider.Object.Name, true));
    }
}
using Notifier.Lambda.Domain.Providers;

namespace Notifier.Lambda.Application.HealthChecks;

public class HealthCheckService : IHealthCheckService
{
    private readonly IEnumerable<ProviderBase> _providerServices;

    public HealthCheckService(IEnumerable<ProviderBase> providerServices)
    {
        _providerServices = providerServices;
    }

    public bool IsHealthy()
    {
        return _providerServices.All(p => p.CheckHealth());
    }

    public Dictionary<string, bool> GetHealthStatus()
    {
        return _providerServices.ToDictionary(p => p.Name, p => p.CheckHealth());
    }
}
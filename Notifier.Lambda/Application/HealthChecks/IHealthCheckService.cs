namespace Notifier.Lambda.Application.HealthChecks
{
    /// <summary>
    /// Interface for a health check service that provides methods to check the overall health status
    /// of the application and retrieve detailed health status of individual components.
    /// </summary>
    public interface IHealthCheckService
    {
        /// <summary>
        /// Checks if the overall system is healthy.
        /// </summary>
        /// <returns>A boolean indicating the overall health status. True if healthy, false otherwise.</returns>
        bool IsHealthy();

        /// <summary>
        /// Retrieves the health status of individual components in the system.
        /// </summary>
        /// <returns>A dictionary where the keys are component names and the values are booleans indicating
        /// the health status of each component. True if the component is healthy, false otherwise.</returns>
        Dictionary<string, bool> GetHealthStatus();
    }
}
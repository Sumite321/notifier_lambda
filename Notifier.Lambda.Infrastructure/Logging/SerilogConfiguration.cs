using Serilog;

namespace Notifier.Lambda.Infrastructure.Logging
{
    namespace NotificationService.Infrastructure.Logging
    {
        public static class SerilogConfigurator
        {
            public static ILogger ConfigureLogger()
            {
                return new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .WriteTo.Console()
                    .CreateLogger();
            }
        }
    }

}

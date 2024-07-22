using Amazon.Lambda.Annotations;
using Amazon.SQS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Notifier.Lambda.Application.HealthChecks;
using Notifier.Lambda.Application.Options;
using Notifier.Lambda.Application.Options.Channel;
using Notifier.Lambda.Application.Options.Provider;
using Notifier.Lambda.Application.Services;
using Notifier.Lambda.Domain.Channels;
using Notifier.Lambda.Domain.Providers;
using Notifier.Lambda.Handlers.Channels;
using Notifier.Lambda.Handlers.Providers;
using Notifier.Lambda.Infrastructure.Redis;
using Serilog;
using Serilog.Formatting.Compact;
using ILogger = Serilog.ILogger;

namespace NotificationServiceLambda;

[LambdaStartup]
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build(); 

        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.Console(new RenderedCompactJsonFormatter())
            .CreateLogger();
            
        configuration.Bind("Channels", new ChannelOptions());
        configuration.Bind("Providers", new ProviderOptions());
            
        // Register Serilog with Microsoft.Extensions.Logging
        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddSerilog(dispose: true);
        });
        services.AddSingleton<IConfiguration>(configuration);
        services.AddSingleton<IAmazonSQS, AmazonSQSClient>();
        services.AddSingleton<IChannel, EmailChannelHandler>();
        services.AddSingleton<IChannel, PushChannelHandler>();
        services.AddSingleton<IChannel, SmsChannelHandler>();
        services.AddSingleton<ProviderBase, TwilioProviderHandler>();
        services.AddSingleton<ProviderBase, SnsProviderHandler>();
        services.AddSingleton<ProviderBase, VonageProviderHandler>();
        services.AddSingleton<IHealthCheckService, HealthCheckService>();
        services.AddSingleton<IRedisService, RedisService>();

        InitializeChannelService(services);
    }
        
    private void InitializeChannelService(IServiceCollection services)
    {
        using var serviceProvider = services.BuildServiceProvider();
        var emailChannel = serviceProvider.GetRequiredService<EmailChannelHandler>();
        var smsChannel = serviceProvider.GetRequiredService<SmsChannelHandler>();
        var pushChannel = serviceProvider.GetRequiredService<PushChannelHandler>();
        var twilioProvider = serviceProvider.GetRequiredService<TwilioProviderHandler>();
        var snsProvider = serviceProvider.GetRequiredService<SnsProviderHandler>();
        var vonageProvider = serviceProvider.GetRequiredService<VonageProviderHandler>();
        var logger = serviceProvider.GetRequiredService<ILogger>();
        var sqsClient = serviceProvider.GetRequiredService<IAmazonSQS>();
                
        var channels = new List<IChannel> { emailChannel, smsChannel, pushChannel };
        var providers = new List<ProviderBase> { twilioProvider, snsProvider, vonageProvider };
                
        services.AddSingleton<IChannelService>(new ChannelService(channels, logger));
        services.AddSingleton<IProviderService>(new ProviderService(providers, logger, sqsClient));
    }
}
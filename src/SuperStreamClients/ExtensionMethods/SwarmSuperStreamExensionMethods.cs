using SuperStreamClients;
using SuperStreamClients.Consumers;
using SuperStreamClients.Producers;
using SuperStreamClients.Analytics;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Extensions.Hosting;
using OpenTelemetry.Trace;

public static class SwarmSuperStreamExensionMethods
{
    public static IServiceCollection AddSwarmSuperStream(
          this IServiceCollection services,
          IConfiguration namedConfigurationSection,
          Action<RabbitMqStreamOptions> config)
    {
        services.AddOpenTelemetry()
            .WithMetrics(builder => builder
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddMeter(
                    "SuperStreamClients.Consumers",
                    "SuperStreamClients.Producers",
                    "SuperStreamClients.Analytics",
                    "SuperStreamClients.RabbitMqStreamConnectionFactory")
                .AddPrometheusExporter())
            .StartWithHost();
        services.Configure<RabbitMqStreamOptions>(namedConfigurationSection);
        services.Configure<RabbitMqStreamOptions>((options)=> {config(options);});
        services.AddSingleton<SuperStreamAnalytics>();
        services.AddSingleton<RabbitMqStreamConnectionFactory>();
        services.AddHostedService<ConsumerBackgroundWorker>();
        services.AddHostedService<ProducerBackgroundWorker>();
        services.AddHostedService<AnalyticsBackgroundWorker>();

        services.AddHttpClient<CustomerAnalyticsClient>((sp, client )=>
        {
            var options = sp.GetService<IOptions<RabbitMqStreamOptions>>();
            var logger = sp.GetService<ILogger<CustomerAnalyticsClient>>();
            var uri = options.Value.AnalyticsApi;
            client.BaseAddress = new Uri(uri + "/CustomerAnalytics");
            logger.LogInformation("Analytics URI set to {analyticsUrl}", client.BaseAddress);
        });
        return services;
    }
}

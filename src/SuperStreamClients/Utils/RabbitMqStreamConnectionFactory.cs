namespace SuperStreamClients;
using Polly;

public class RabbitMqStreamConnectionFactory
{
    ILogger<RabbitMqStreamConnectionFactory> _logger;
    IOptions<RabbitMqStreamOptions> _options;

    public RabbitMqStreamConnectionFactory(
        ILogger<RabbitMqStreamConnectionFactory> logger,
        IOptions<RabbitMqStreamOptions> options)
    {
        _logger = logger;
        _options = options;
    }


    public async Task<StreamSystem> Create(CancellationToken cancellationToken)
    {
        try
        {
            return await CreateWithRetry(cancellationToken);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning("Cancelled RabbitMq connection attempts.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to Connect to RabbitMq");
            throw;
        }
    }

    private async Task<StreamSystem> CreateWithRetry(CancellationToken cancellationToken)
    {
        var retryTime = _options.Value.RabbitMqConnectionRetryTime;

        return await Policy.Handle<StreamSystemInitialisationException>()
            .WaitAndRetryAsync(
                int.MaxValue,
                (count) => retryTime,
                onRetry: async (ex, delay, attempt, context) =>
                {
                    _logger.LogInformation("Connection will retry in {connectionRetry}", delay);
                    await Task.Delay(delay, cancellationToken);
                    _logger.LogInformation("Connection retry {attempt}", attempt);
                })
            .ExecuteAsync(async (ct) => await AttemptCreateConfigAndStreamSystem(ct), cancellationToken);
    }


    private async Task<StreamSystem> AttemptCreateConfigAndStreamSystem(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            return await CreateConfigAndStreamSystem(cancellationToken);
        }
        catch (StreamSystemInitialisationException ex)
        {
            _logger.LogWarning("Failed to connect to host: {StreamSystemInitialisationException}.", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to connect to host.");
            throw;
        }
    }

    private async Task<StreamSystem> CreateConfigAndStreamSystem(CancellationToken cancellationToken)
    {
        var config = await GetConnectionConfig();
        var streamSystem = await StreamSystem.Create(config);
        return streamSystem;
    }

    public async Task<StreamSystemConfig> GetConnectionConfig()
    {
        var options = _options.Value;
        _logger.LogInformation("{user} Connecting to {hostname} {virtualHost}", options.UserName, options.HostName, options.VirtualHost);
        var hostEntry = await Dns.GetHostEntryAsync(options.HostName);
        _logger.LogInformation("{IpCount} IP Addresses found", hostEntry.AddressList.Length);
        var config = new StreamSystemConfig
        {
            UserName = options.UserName,
            Password = options.Password,
            VirtualHost = options.VirtualHost,
            Endpoints = GetEndpoints(hostEntry, options.StreamPort).ToList()
        };
        return config;
    }

    private IEnumerable<EndPoint> GetEndpoints(IPHostEntry hostEntry, int streamPort)
    {
        var endpoints = hostEntry.AddressList.Select(address => new IPEndPoint(address, streamPort));
        foreach (var endpoint in endpoints)
        {
            _logger.LogInformation("RabbitMq endpoint {endpoint}:{port}", endpoint.Address, endpoint.Port);
            yield return (EndPoint)endpoint;
        }
    }
}
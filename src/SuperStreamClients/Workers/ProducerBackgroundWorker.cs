using System.Text;
using RabbitMQ.Stream.Client.AMQP;
using RabbitMQ.Stream.Client.Reliable;

namespace SuperStreamClients.Producers;

public class ProducerBackgroundWorker : BackgroundService
{
    private readonly ILogger<ProducerBackgroundWorker> _logger;
    private readonly RabbitMqStreamConnectionFactory _systemConnection;
    private readonly IOptions<RabbitMqStreamOptions> _options;
    private readonly Random _random;
    StreamSystem _streamSystem;
    Producer _producer;

    public ProducerBackgroundWorker(
        ILogger<ProducerBackgroundWorker> logger,
        RabbitMqStreamConnectionFactory systemConnection,
        IOptions<RabbitMqStreamOptions> options)
    {
        _logger = logger;
        _systemConnection = systemConnection;
        _options = options;
        var optionsValue = _options.Value;
        _random = new Random(optionsValue.RandomSeed);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!IsProducerEnabled())
            return;

        await TryCreateConnectionAndStartSendingMesages(stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await StopProducerAndConnection();
        await base.StopAsync(cancellationToken);
    }

    private async Task StopProducerAndConnection()
    {
        if (!IsProducerEnabled())
            return;

        if (_producer != null)
            await _producer.Close();

        if (_streamSystem != null)
            await _streamSystem.Close();
    }

    private bool IsProducerEnabled() => _options.Value.Producer;

    private async Task TryCreateConnectionAndStartSendingMesages(CancellationToken cancellationToken)
    {
        try
        {
            await CreateConnectionAndStartSendingMesages(cancellationToken);
        }
        catch (TaskCanceledException tce)
        {
            _logger.LogWarning("Producer requested to stop.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failure. Producer worker terminating.");
        }
    }

    private async Task CreateConnectionAndStartSendingMesages(CancellationToken cancellationToken)
    {
        await CreateConnection(cancellationToken);
        await CreateProducer(cancellationToken);
        await SendMessages(cancellationToken);
    }

    private async Task SendMessages(CancellationToken cancellationToken)
    {
        var options = _options.Value;
        for (int messageId = 0; messageId < options.ProducerMessageSendLimit; messageId++)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            await SendMessageAndDelay(messageId, cancellationToken);
        };
    }

    private async Task SendMessageAndDelay(int messageId, CancellationToken cancellationToken)
    {
        var options = _options.Value;
        await SendMessage(messageId);
        await DelayNextSend(options, cancellationToken);
    }

    private async Task SendMessage(int messageNumber)
    {
        var options = _options.Value;
        var message = CreateMessage(messageNumber, options);
        await _producer.Send(message);
        _logger.LogInformation("Written Message {messageNumber}", messageNumber);
    }

    private async Task DelayNextSend(RabbitMqStreamOptions options, CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(
                _random.Next(options.ProducerSendDelayMin, options.ProducerSendDelayMax),
                cancellationToken);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError("Delay canceled due cancellation requested");
        }
    }

    private Message CreateMessage(int messageNumber, RabbitMqStreamOptions options)
    {
        var customerId = _random.Next(0, options.NumberOfCustomers);
        var orderId = Guid.NewGuid();
        var message = new Message(SerializeProducedMessage(messageNumber, customerId, orderId))
        {
            Properties = new Properties() { MessageId = CreateMessageId(customerId) }
        };
        return message;
    }

    private string CreateMessage(int messageNumber, int customerId, Guid orderId) =>
        $"Some message about the order here";

    private byte[] SerializeProducedMessage(int messageNumber, int customerId, Guid orderId)
    {
        var message = CreateProducedMessage(messageNumber, customerId, orderId);
        return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(message);
    }

    private CustomerMessage CreateProducedMessage(int messageNumber, int customerId, Guid orderId)
    {
        var hostName = GetHostName();
        var data = CreateMessage(messageNumber, customerId, orderId);

        var message = new CustomerMessage(
            hostName,
            customerId,
            orderId,
            messageNumber,
            data);

        return message;
    }

    private byte[] SerializeProducedMessage(CustomerMessage message)
    {
        return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(message);
    }

    private string GetHostName() => System.Environment.MachineName;

    private string CreateMessageId(int customerId) =>
        customerId.ToString();

    private async Task CreateConnection(CancellationToken cancellationToken)
    {
        _streamSystem = await _systemConnection.Create(cancellationToken);
    }

    private async Task CreateProducer(CancellationToken cancellationToken)
    {
        var options = _options.Value;

        _producer = await Producer.Create(new ProducerConfig(_streamSystem, options.StreamName)
        {
            SuperStreamConfig = new SuperStreamConfig()
            {
                Routing = message1 => message1.Properties.MessageId.ToString()
            }
        });
    }
}

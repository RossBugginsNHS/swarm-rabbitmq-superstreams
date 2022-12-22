using System.Buffers;
using System.Text;
using RabbitMQ.Stream.Client.AMQP;
using RabbitMQ.Stream.Client.Reliable;

namespace SuperStreamClients.Consumers;
public class ConsumerBackgroundWorker : BackgroundService
{
    private readonly ILogger<ConsumerBackgroundWorker> _logger;
    private readonly RabbitMqStreamConnectionFactory _systemConnection;
    private readonly IOptions<RabbitMqStreamOptions> _options;
    private readonly Random _random;
    private readonly CustomerAnalyticsClient _analyticsClient;
    StreamSystem _streamSystem;
    Consumer _consumer;

    public ConsumerBackgroundWorker(
        ILogger<ConsumerBackgroundWorker> logger,
        RabbitMqStreamConnectionFactory systemConnection,
        IOptions<RabbitMqStreamOptions> options,
        CustomerAnalyticsClient analyticsClient
    )
    {
        _logger = logger;
        _systemConnection = systemConnection;
        _options = options;
        _analyticsClient = analyticsClient;
        var optionsValue = _options.Value;
        _random = new Random(optionsValue.RandomSeed);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!IsConsumerEnabled())
            return;

        await TryCreateConnectionAndConsumer(stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await StopConsumerAndConnection();
        await base.StopAsync(cancellationToken);
    }

    private async Task StopConsumerAndConnection()
    {
        if (!IsConsumerEnabled())
            return;

        if (_consumer != null)
            await _consumer.Close();

        if (_streamSystem != null)
            await _streamSystem.Close();
    }

    private bool IsConsumerEnabled() => _options.Value.Consumer;

    private async Task CreateConnection(CancellationToken cancellationToken)
    {
        _streamSystem = await _systemConnection.Create(cancellationToken);
    }

    private async Task TryCreateConnectionAndConsumer(CancellationToken cancellationToken)
    {
        try
        {
            await CreateConnectionAndConsumer(cancellationToken);
        }
        catch (TaskCanceledException tce)
        {
            _logger.LogWarning("Consumer requested to stop.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failure. Consumer worker terminating.");
        }
    }

    private async Task CreateConnectionAndConsumer(CancellationToken cancellationToken)
    {
        await CreateConnection(cancellationToken);
        await CreateConsumer(cancellationToken);
    }

    public async Task<Consumer> CreateConsumer(CancellationToken cancellationToken)
    {
        var options = _options.Value;
        var consumerConfig = CreateConsumerConfig(cancellationToken);
        _consumer = await Consumer.Create(consumerConfig);
        _logger.LogInformation("Consumer created");
        return _consumer;
    }

    private ConsumerConfig CreateConsumerConfig(
        CancellationToken cancellationToken)
    {
        var options = _options.Value;
        return new ConsumerConfig(_streamSystem, options.StreamName)
        {
            IsSuperStream = true,
            IsSingleActiveConsumer = true,
            Reference = options.ConsumerAppReference,
            OffsetSpec = new OffsetTypeNext(),
            MessageHandler = async (sourceStream, consumer, ctx, message) =>
            {
                await TryMessageHandle(sourceStream, consumer, ctx, message, cancellationToken);
            }
        };
    }

    public virtual async Task TryMessageHandle(
        string sourceStream,
        RawConsumer consumer,
        MessageContext ctx,
        Message message,
        CancellationToken cancellationToken)
    {
        try
        {
            await MessageHandle(sourceStream, consumer, ctx, message, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Consumer failed handling message.");
        }
    }

    public virtual async Task MessageHandle(
        string sourceStream,
        RawConsumer consumer,
        MessageContext ctx,
        Message message,
        CancellationToken cancellationToken)
    {
        var receivedMessage = CreateReceivedCustomerMessage(sourceStream, message);
        LogMessageReceived(sourceStream, receivedMessage);
        await PostAnalyticsAndDelay(receivedMessage, cancellationToken);
    }

    private async Task PostAnalyticsAndDelay(ReceivedCustomerMessage receivedMessage, CancellationToken cancellationToken)
    {
        var options = _options.Value;
        var postAnalticsTask = PostAnalytics(receivedMessage);
        var delayTask = DelayMessageHandling(options, cancellationToken);
        await Task.WhenAll(postAnalticsTask, delayTask);
    }

    private string GetHostName() => System.Environment.MachineName;

    private int GetCustomerId(Message message)
    {
        var customerIdString = message.Properties.MessageId.ToString();
        var customerId = int.Parse(customerIdString);
        return customerId;
    }

    private void LogMessageReceived(
        string sourceStream,
        ReceivedCustomerMessage data)
    {
        var hostName = GetHostName();
        var customerId = data.CustomerMessage.CustomerId;
        _logger.LogInformation(
            "Message {messageNumber}, Received for {customerId}.",
            data.CustomerMessage.MessageNumber,
            customerId);
    }

    private ReceivedCustomerMessage CreateReceivedCustomerMessage(
        string sourceStream,
        Message rabbitmqMessage)
    {
        var customerMessage = DecodeProducedData(rabbitmqMessage);
        return CreateReceivedCustomerMessage(sourceStream, customerMessage);
    }

    private ReceivedCustomerMessage CreateReceivedCustomerMessage(
        string sourceStream,
        CustomerMessage sentMessage)
    {
        var hostName = GetHostName();
        return new ReceivedCustomerMessage(
            TimeOnly.FromDateTime(DateTime.Now),
            hostName,
            sourceStream,
            sentMessage);
    }

    private CustomerMessage DecodeProducedData(Message message)
    {
        var data = message.Data.Contents.ToArray();
        return System.Text.Json.JsonSerializer.Deserialize<CustomerMessage>(data);
    }

    private async Task PostAnalytics(ReceivedCustomerMessage message)
    {
        await _analyticsClient.Post(message);
    }

    private async Task DelayMessageHandling(
        RabbitMqStreamOptions options,
        CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(
                _random.Next(options.ConsumerHandleDelayMin, options.ConsumerHandleDelayMax),
                cancellationToken);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError("Handle delay canceled due cancellation requested");
        }
    }
}
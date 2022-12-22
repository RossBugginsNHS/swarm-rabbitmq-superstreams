using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Mvc;
using SuperStreamClients.Analytics;
namespace SuperStreamClients.Analytics;

[ApiController]
[Route("[controller]")]
public class CustomerAnalyticsController : ControllerBase
{
    static Meter s_meter = new Meter("SuperStreamClients.Analytics", "1.0.0");
    static Counter<int> s_messagesReceived = s_meter.CreateCounter<int>("messages-received-count");

    private readonly ILogger<CustomerAnalyticsController> _logger;
    private readonly SuperStreamAnalytics _analytics;
    public CustomerAnalyticsController(
        ILogger<CustomerAnalyticsController> logger,
        SuperStreamAnalytics analytics)
    {
        _logger = logger;
        _analytics = analytics;
    }

    [HttpPost(Name = "PostCustomerMessage")]
    public ReceivedCustomerMessageResponse Post(ReceivedCustomerMessage message, CancellationToken ct = default(CancellationToken))
    {
        UpdateMetrics(message.CustomerMessage.CustomerId, message.ReceivedOnHost);
        _analytics.NewMessage(message);
        _logger.LogInformation("Analytics got message {message}", message.ToString());
        var response = new ReceivedCustomerMessageResponse(
            Host: Environment.MachineName,
            Message: $"Thanks for message {message.CustomerMessage.MessageNumber}");
        return response;
    }

    [HttpGet("{customerId}", Name = "GetCustomerMessages")]
    public IEnumerable<ReceivedCustomerMessage> Get(int customerId, CancellationToken cancellationToken = (default))
    {
        return _analytics.GetMessagesForCustomer(customerId, cancellationToken);
    }

    [HttpGet(Name = "GetCustomerMessageSummary")]
    public MessageSummary Get(CancellationToken cancellationToken = (default))
    {
        return _analytics.GetMessageSummary(cancellationToken);
    }

    private void UpdateMetrics(int customerId, string hostName)
    {
        s_messagesReceived.Add(
            1,
            new KeyValuePair<string, object>("Host", hostName),
            new KeyValuePair<string, object>("CustomerId", customerId));
    }
}

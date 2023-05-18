namespace Consumer.API.EventHandlers;

using System.Threading.Tasks;

using Azure.Messaging.ServiceBus;

using Cloud.Azure.ServiceBus;

using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;

using Platform.Config;
using Platform.Messages;

public class OrderMessageHandler : ServiceBusMessageProcessor<OrderCreated>
{
    private readonly PlatformServiceBusOptions _ServiceBusOptions;

    public OrderMessageHandler(
        IAzureClientFactory<ServiceBusClient> serviceBusClientFactory,
        ILogger<OrderMessageHandler> logger,
        IOptions<PlatformServiceBusOptions> serviceBusOptions) : base(serviceBusClientFactory, logger)
    {
        _ServiceBusOptions = serviceBusOptions.Value;
    }

    protected override String Namespace => _ServiceBusOptions.Namespace.SampleApp;
    protected override String QueueOrTopicName => _ServiceBusOptions.QueueName.OrderCreated;

    protected override Task HandleMessageAsync(OrderCreated orderCreatedMessage)
    {
        ArgumentNullException.ThrowIfNull(orderCreatedMessage, nameof(orderCreatedMessage));

        if (orderCreatedMessage.Amount <= 0)
        {
            _Logger.LogError($"Order amount: {orderCreatedMessage.Amount} is invalid");
            throw new InvalidOperationException("amount should be greated than 0");
        }

        _Logger.LogInformation($"Order received: {orderCreatedMessage}");
        return Task.CompletedTask;
    }
}

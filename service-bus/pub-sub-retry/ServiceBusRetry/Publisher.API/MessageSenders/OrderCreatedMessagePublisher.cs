namespace Publisher.API.MessageSenders;

using Azure.Messaging.ServiceBus;
using Cloud.Azure.ServiceBus;
using Microsoft.Extensions.Azure;

using Microsoft.Extensions.Options;

using Platform.Config;
using Platform.Messages;

public class OrderCreatedMessagePublisher : ServiceBusMessageSender<OrderCreated>
{
    private readonly PlatformServiceBusOptions _ServiceBusOptions;

    public OrderCreatedMessagePublisher(
        IAzureClientFactory<ServiceBusClient> serviceBusClientFactory,
        ILogger<OrderCreatedMessagePublisher> logger,
        IOptions<PlatformServiceBusOptions> serviceBusOptions) : base(serviceBusClientFactory, logger)
    {
        _ServiceBusOptions = serviceBusOptions.Value;
    }

    protected override String Namespace => _ServiceBusOptions.Namespace.SampleApp;
    protected override String QueueOrTopicName => _ServiceBusOptions.TopicName.OrderCreated;
}

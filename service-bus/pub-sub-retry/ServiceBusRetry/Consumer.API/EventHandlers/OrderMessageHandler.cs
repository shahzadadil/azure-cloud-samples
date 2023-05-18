namespace Consumer.API.EventHandlers;

using System.Text.Json;

using Azure.Messaging.ServiceBus;

using Cloud.Azure.ServiceBus;

using Platform.Messages;

public class OrderMessageHandler : IServiceBusMessageHandler
{
    private readonly ILogger<OrderMessageHandler> _Logger;

    public OrderMessageHandler(ILogger<OrderMessageHandler> logger)
    {
        _Logger = logger;
    }

    public void Handle(ServiceBusReceivedMessage message)
    {
        var orderMessage = JsonSerializer.Deserialize<OrderCreated>(message.Body.ToString());

        ArgumentNullException.ThrowIfNull(orderMessage, nameof(orderMessage));

        if (orderMessage.Amount <= 0)
        {
            _Logger.LogError($"Order amount: {orderMessage.Amount} is invalid");
            throw new InvalidOperationException("amount should be greated than 0");
        }

        _Logger.LogInformation($"Order received: {orderMessage}");
    }
}

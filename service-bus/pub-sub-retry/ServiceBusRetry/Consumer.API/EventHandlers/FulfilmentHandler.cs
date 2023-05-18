namespace Consumer.API.EventHandlers;

using System.Text.Json;

using Azure.Messaging.ServiceBus;

using Cloud.Azure.ServiceBus;

using Platform.Messages;

public class FulfilmentHandler : IServiceBusMessageHandler
{
    private readonly ILogger<FulfilmentHandler> _Logger;

    public FulfilmentHandler(ILogger<FulfilmentHandler> logger)
    {
        _Logger = logger;
    }

    public void Handle(ServiceBusReceivedMessage message)
    {
        var orderMessage = JsonSerializer.Deserialize<OrderCreated>(message.Body.ToString());

        if (orderMessage is null)
            throw new ArgumentNullException(nameof(orderMessage));

        if (orderMessage.Amount <= 0)
        {
            _Logger.LogError($"Order amount: {orderMessage.Amount} is invalid");
            throw new InvalidOperationException("amount should be greated than 0");
        }

        _Logger.LogInformation($"Fulfilment received: {orderMessage}");
    }
}

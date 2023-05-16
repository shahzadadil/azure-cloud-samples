namespace Consumer.API;

using System.Text.Json;
using System.Threading.Tasks;

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

    public Task HandleAsync(ServiceBusReceivedMessage message)
    {
        var orderCreatedMessage = JsonSerializer.Deserialize<OrderCreated>(
            message.Body.ToString()) ?? new();

        _Logger.LogInformation($"Message received: {orderCreatedMessage}");

        if (orderCreatedMessage.Amount <= 0)
        {
            throw new InvalidOperationException();
        }

        return Task.CompletedTask;
    }
}

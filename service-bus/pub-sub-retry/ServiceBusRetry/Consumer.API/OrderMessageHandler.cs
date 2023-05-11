namespace Consumer.API;

using System.Text.Json;
using System.Threading.Tasks;

using Azure.Messaging.ServiceBus;

using Cloud.Azure.ServiceBus;

public class OrderMessageHandler : IServiceBusMessageHandler
{
    private readonly ILogger<OrderMessageHandler> _Logger;

    public OrderMessageHandler(ILogger<OrderMessageHandler> logger)
    {
        _Logger = logger;
    }

    public Task HandleAsync(ServiceBusReceivedMessage message)
    {
        var messageContent = message.Body.ToString();
        _Logger.LogInformation($"Message received: {messageContent}");
        return Task.CompletedTask;
    }
}

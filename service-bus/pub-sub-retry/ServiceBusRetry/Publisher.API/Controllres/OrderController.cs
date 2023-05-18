// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Publisher.API.Controllres;

using System.Text;
using System.Text.Json;

using Azure.Messaging.ServiceBus;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;

using Platform.Config;
using Platform.Messages;

using Publisher.API.Models;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IAzureClientFactory<ServiceBusClient> _ServiceBusClientFactory;
    private readonly PlatformOptions _PlatformOptions;

    public OrderController(
        IAzureClientFactory<ServiceBusClient> serviceBusClientFactory,
        IOptions<PlatformOptions> platformOptions)
    {
        _ServiceBusClientFactory = serviceBusClientFactory;
        _PlatformOptions = platformOptions.Value;
    }

    [HttpPost("queue")]
    public async Task Post([FromBody] OrderModel orderModel)
    {
        var serviceBusClient = _ServiceBusClientFactory.CreateClient("SampleAppServiceBusClient");
        var sender = serviceBusClient.CreateSender(_PlatformOptions.ServiceBus.QueueName.OrderCreated);

        OrderCreated message = new()
        {
            OrderId = orderModel.Id,
            Amount = orderModel.Amount,
            CreatedOn = DateTime.UtcNow,
        };

        ServiceBusMessage serviceBusMessage = new(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message)));

        await sender.SendMessageAsync(serviceBusMessage);
    }

    [HttpPost("topic")]
    public async Task PostToTopic([FromBody] OrderModel orderModel)
    {
        var serviceBusClient = _ServiceBusClientFactory.CreateClient("SampleAppServiceBusClient");

        var sender = serviceBusClient.CreateSender(
            _PlatformOptions.ServiceBus.TopicName.OrderCreated);

        OrderCreated message = new()
        {
            OrderId = orderModel.Id,
            Amount = orderModel.Amount,
            CreatedOn = DateTime.UtcNow,
        };

        ServiceBusMessage serviceBusMessage = new(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message)));

        await sender.SendMessageAsync(serviceBusMessage);
    }

}

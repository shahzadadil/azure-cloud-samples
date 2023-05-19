// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Publisher.API.Controllres;

using Microsoft.AspNetCore.Mvc;

using Platform.Messages;

using Publisher.API.MessageSenders;
using Publisher.API.Models;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly OrderCreatedMessageSender _OrderCreatedMessageSender;
    private readonly OrderCreatedMessagePublisher _OrderCreatedMessagePublisher;

    public OrderController(
        OrderCreatedMessageSender orderCreatedMessageSender,
        OrderCreatedMessagePublisher orderCreatedMessagePublisher)
    {
        _OrderCreatedMessageSender = orderCreatedMessageSender;
        _OrderCreatedMessagePublisher = orderCreatedMessagePublisher;
    }

    [HttpPost("queue")]
    public async Task Post([FromBody] OrderModel orderModel)
    {
        OrderCreated message = new()
        {
            OrderId = orderModel.Id,
            Amount = orderModel.Amount,
            CreatedOn = DateTime.UtcNow,
        };

        await _OrderCreatedMessageSender.SchdeuleMessageAsync(
            message,
            DateTimeOffset.UtcNow.AddSeconds(orderModel.ScheduleOffsetSeconds));
    }

    [HttpPost("topic")]
    public async Task PostToTopic([FromBody] OrderModel orderModel)
    {
        OrderCreated message = new()
        {
            OrderId = orderModel.Id,
            Amount = orderModel.Amount,
            CreatedOn = DateTime.UtcNow,
        };

        await _OrderCreatedMessagePublisher.SchdeuleMessageAsync(
            message,
            DateTimeOffset.UtcNow.AddSeconds(orderModel.ScheduleOffsetSeconds));
    }

}

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MSA.Shipping.API.Controllers;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly ILogger<OrderController> _logger;

    public OrderController(ILogger<OrderController> logger)
    {
        _logger = logger;
    }

    // GET: api/<ShippingController>
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new string[] { "value1", "value2" };
    }

    // GET api/<ShippingController>/5
    [HttpGet("{id}")]
    public string Get(int id)
    {
        return "value";
    }

    // POST api/<ShippingController>
    [HttpPost]
    public Guid Post()
    {
        var orderId = Guid.NewGuid();
        _logger.LogInformation($"Order {orderId} created");
        return orderId;
    }

    // PUT api/<ShippingController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<ShippingController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}

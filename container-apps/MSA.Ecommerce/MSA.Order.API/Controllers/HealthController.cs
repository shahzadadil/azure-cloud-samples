namespace MSA.Order.API.Controllers;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{

    [HttpGet]
    public OkResult Get()
    {
        return this.Ok();
    }
}

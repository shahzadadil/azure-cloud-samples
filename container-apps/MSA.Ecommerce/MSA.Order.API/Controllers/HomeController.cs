namespace MSA.Order.API.Controllers;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("")]
public class HomeController : ControllerBase
{

    [HttpGet]
    public string Get()
    {
        return "Order API is working";
    }
}

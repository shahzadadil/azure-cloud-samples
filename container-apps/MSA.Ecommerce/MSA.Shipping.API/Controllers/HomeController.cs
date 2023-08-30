namespace MSA.Shipping.API.Controllers;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("")]
public class HomeController : ControllerBase
{

    [HttpGet]
    public string Get()
    {
        return "Shipping API is working";
    }
}

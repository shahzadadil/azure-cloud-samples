namespace Resource.API.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[Route("[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    private readonly ILogger<TestController> _logger;

    public TestController(ILogger<TestController> logger)
    {
        _logger = logger;
    }

    [HttpGet("sample")]
    public ActionResult Sample()
    {
        if(!HttpContext.Request.Headers.ContainsKey("x-dom-access"))
        {
            _logger.LogError("x-dom-access header is not present");
            return Forbid();
        }

        var userHeader = HttpContext.Request.Headers["x-dom-access"].ToString();

        if (userHeader is null || String.IsNullOrEmpty(userHeader))
        {
            _logger.LogError("x-dom-access header is null or empty");
            return new ForbidResult();
        }

        _logger.LogInformation($"x-dom-access header is present {userHeader}");
        return Ok($"The auth controller added {userHeader} as the user name");
    }
}

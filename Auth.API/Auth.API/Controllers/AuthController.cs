namespace Auth.API.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;

    public AuthController(ILogger<AuthController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public ActionResult VerifyAccess()
    {
        var authHeader = HttpContext.Request.Headers["Authorization"].ToString();

        if (authHeader is null || String.IsNullOrEmpty(authHeader))
        {
            _logger.LogError("Authorization header is not present");
            return new ForbidResult();
        }

        _logger.LogInformation($"Authorization header {authHeader} present");
        HttpContext.Items["x-dom-access"] = "SampleUserName";
        HttpContext.Response.Headers.Add("x-dom-access", "SampleUserName");
        _logger.LogInformation("Added x-dom-acces header to response");

        return Ok();
    }
}

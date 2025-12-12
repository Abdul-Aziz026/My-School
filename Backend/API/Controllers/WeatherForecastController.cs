using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
//[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    public WeatherForecastController()
    {
        
    }

    [HttpGet("")]
    public IActionResult Greet()
    {
        return Ok("Hello, World!");
    }
}

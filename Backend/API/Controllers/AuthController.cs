using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AuthController : Controller
{
    [HttpPost("register")]
    public IActionResult Register()
    {
        return Ok("register");
    }

    [HttpPost("login")]
    public IActionResult Login()
    {
        return Ok("login");
    }
    [HttpPost("refresh")]
    public IActionResult Refresh()
    {
        return Ok("refresh");
    }
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return Ok("logout");

    }
    [HttpGet("me")]
    public IActionResult Profile()
    {
        return Ok("Profile");
    }
}

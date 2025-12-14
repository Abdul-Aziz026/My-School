using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AuthController : Controller
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    [HttpGet("register")]
    //[HttpPost("register")]
    public Task<string> Register(RegisterDto user)
    {
        return _authService.RegisterAsync(user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginDto user)
    {
        var response = await _authService.LoginAsync(user);
        if (response is null)
        {
            return BadRequest("Email or Password not mached...");
        }
        var result = new AuthResponse()
        {
            Email = user.Email,
            Token = response,
        };
        return Ok(response);
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

using Application.Helper;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

/// <summary>
/// Test endpoints for different roles
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [Authorize]
    [HttpGet("debug-auth")]
    public IActionResult DebugAuth()
    {
        Console.WriteLine(TellMe.UserId);
        return Ok(new
        {
            IsAuthenticated = User.Identity?.IsAuthenticated,
            AuthenticationType = User.Identity?.AuthenticationType,
            Name = User.Identity?.Name,
            Roles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList(),
            AllClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
        });
    }
    /// <summary>
    /// User-only endpoint
    /// </summary>
    [Authorize(Roles = "User")]
    [Authorize(Policy = "CanViewUsers")]
    [HttpGet("user")]
    public IActionResult UserEndpoint() => Ok(new { message = "User access granted" });

    /// <summary>
    /// Admin-only endpoint
    /// </summary>
    //[Authorize(Roles = "Admin")]
    [Authorize]
    [HttpGet("admin")]
    public IActionResult AdminEndpoint() => Ok(new { message = "Admin access granted" });

    /// <summary>
    /// Admin or Manager endpoint
    /// </summary>
    [Authorize(Roles = "Admin,Manager")]
    [HttpGet("admin-or-manager")]
    public IActionResult AdminOrManager() => Ok(new { message = "Admin/Manager access" });
}

using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/mcp/[controller]")]
public sealed class McpToolsController : ControllerBase{
    [HttpGet("mcp")]
    public IActionResult Greet()
    {
        return Ok();
    }
}

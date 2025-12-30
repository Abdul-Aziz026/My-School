using Application.Interfaces.Publisher;
using Contracts.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly IMessageBus _bus;

    public NotificationController(IMessageBus bus)
    {
        _bus = bus;
    }

    [HttpPost("notify-students")]
    public async Task<IActionResult> NotifyStudents([FromBody] NotifyStudentsCommand command)
    {
        // Send command through mediator
        await _bus.SendAsync<NotifyStudentsCommand, Unit>(command);
        return Ok("Command sent successfully");
    }
}

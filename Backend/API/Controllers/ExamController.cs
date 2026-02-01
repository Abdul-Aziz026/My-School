using Application.Common.Interfaces.Publisher;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExamController : Controller
{
    private readonly IMessageBus _messageBus;

    public ExamController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public IActionResult Index()
    {
        return Ok();
    }
}

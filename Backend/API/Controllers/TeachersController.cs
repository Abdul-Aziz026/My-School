using Application.Common.Interfaces.Publisher;
using Application.Features.SchoolClassManagement.DTOs;
using Application.Features.SchoolClassManagement.Queries.GetTeacherClasses;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeachersController : Controller
{
    private readonly IMessageBus _messageBus;
    public TeachersController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }
    /// <summary>
    /// Gets all classes taught by a teacher
    /// </summary>
    [HttpGet("teacher/{teacherId}")]
    public async Task<IActionResult> GetTeacherClasses(string teacherId)
    {
        var query = new GetTeacherClassesQuery(teacherId);
        var result = await _messageBus.SendAsync<GetTeacherClassesQuery, List<ClassResponseDto>>(query);
        return Ok(result);
    }
}

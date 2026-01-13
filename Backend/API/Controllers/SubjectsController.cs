using Application.Common.Interfaces.Publisher;
using Application.Features.SchoolClassManagement.Commands.CreateClass;
using Application.Features.SchoolClassManagement.Commands.CreateSubject;
using Application.Features.SchoolClassManagement.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubjectsController : Controller
{
    private IMessageBus _messageBus;
    public SubjectsController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    /// <summary>
    /// Creates a new subject
    /// </summary>
    /// <returns>Created class ID and location header</returns>
    [HttpPost]
    public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectDto requestDto)
    {
        if (!ModelState.IsValid)
        {
            var errorList = ModelState.Values.SelectMany(v => v.Errors).Select(m => m.ErrorMessage).ToList();
            return BadRequest(errorList);
        }
        var command = requestDto.ToCreateSubjectCommand();
        var responseId = await _messageBus.SendAsync<CreateSubjectCommand, string>(command);
        return CreatedAtAction(null, new { Id = responseId });
    }
}

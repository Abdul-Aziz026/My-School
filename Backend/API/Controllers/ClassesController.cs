
using Application.Common.Interfaces.Publisher;
using Application.Features.SchoolClassManagement.Commands.CreateClass;
using Application.Features.SchoolClassManagement.Commands.DeleteClass;
using Application.Features.SchoolClassManagement.Commands.UpdateClass;
using Application.Features.SchoolClassManagement.Queries.GetClassStudents;
using Application.Features.SchoolClassManagement.Queries.GetClassById;
using Application.Features.SchoolClassManagement.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClassesController : Controller
{
    private readonly IMessageBus _messageBus;
    public ClassesController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    /// <summary>
    /// Creates a new class
    /// </summary>
    /// <returns>Created class ID and location header</returns>
    [HttpPost]
    public async Task<IActionResult> CreateClass([FromBody] CreateClassDto dto)
    {
        var command = dto.ToCreateClassCommand();
        var responseId = await _messageBus.SendAsync<CreateClassCommand, string>(command);
        return CreatedAtAction(null, new { Id = responseId });
    }

    /// <summary>
    /// Gets class by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetClassById(string id)
    {
        var query = new GetClassByIdQuery(id);
        var result = await _messageBus.SendAsync<GetClassByIdQuery, ClassResponseDto>(query);
        return Ok(result);
    }

    /// <summary>
    /// Updates an existing class
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateClass(string id, [FromBody] UpdateClassDto dto)
    {
        var command = dto.ToUpdateClassCommand(id);
        await _messageBus.SendAsync<UpdateClassCommand>(command);

        // 204 No Content - standard for successful updates with no body
        return NoContent();
    }

    /// <summary>
    /// Soft deletes a class
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClass(string id)
    {
        var command = new DeleteClassCommand(id);
        await _messageBus.SendAsync<DeleteClassCommand>(command);
        return NoContent();
    }

    /// <summary>
    /// Gets class with enrolled students
    /// </summary>
    [HttpGet("{id}/students")]
    public async Task<IActionResult> GetClassStudents(string id)
    {
        var query = new GetClassStudentsQuery(id);
        var result = await _messageBus.SendAsync<GetClassStudentsQuery, List<StudentResponseDto>> (query);
        return Ok(result);
    }
}


using Application.Common.Interfaces.Publisher;
using Application.Features.SchoolClassManagement.Commands.RemoveTeacher;
using Application.Features.SchoolClassManagement.Queries.GetClassTeachers;
using Microsoft.AspNetCore.Mvc;
using Application.Features.SchoolClassManagement.StudentManagement.DTOs;
using Application.Features.SchoolClassManagement.TeacherManagement.Commands.AssignTeacher;
using Application.Features.SchoolClassManagement.TeacherManagement.DTOs;
using Application.Features.SchoolClassManagement.ClassManagement.Commands.CreateClass;
using Application.Features.SchoolClassManagement.ClassManagement.Commands.UpdateClass;
using Application.Features.SchoolClassManagement.ClassManagement.Commands.DeleteClass;
using Application.Features.SchoolClassManagement.ClassManagement.Queries.GetAllClasses;
using Application.Features.SchoolClassManagement.ClassManagement.Queries.GetClassById;
using Application.Features.SchoolClassManagement.ClassManagement.Queries.GetClassStudents;
using Application.Features.SchoolClassManagement.ClassManagement.DTOs;

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
    /// Gets all classes
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllClasses()
    {
        var query = new GetAllClassesQuery();
        var result = await _messageBus.SendAsync<GetAllClassesQuery, List<ClassResponseDto>>(query);
        return Ok(result);
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
        return CreatedAtAction(
            nameof(GetClassById),
            new { id = responseId },
            new { Id = responseId }
        );
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
        var result = await _messageBus.SendAsync<GetClassStudentsQuery, List<StudentResponseDto>>(query);
        return Ok(result);
    }

    /// <summary>
    /// Gets teachers assigned to a class
    /// </summary>
    [HttpGet("{id}/teachers")]
    public async Task<IActionResult> GetClassTeachers(string id)
    {
        var query = new GetClassTeachersQuery(id);
        var result = await _messageBus.SendAsync<GetClassTeachersQuery, List<TeacherResponseDto>>(query);
        return Ok(result);
    }

    /// <summary>
    /// Assigns a teacher to a class
    /// </summary>
    [HttpPost("{id}/assign-teacher")]
    public async Task<IActionResult> AssignTeacher(string id, [FromBody] AssignTeacherDto dto)
    {
        var command = new AssignTeacherCommand(id, dto.TeacherId);
        await _messageBus.SendAsync<AssignTeacherCommand>(command);
        return NoContent();
    }

    /// <summary>
    /// Removes a teacher from a class
    /// </summary>
    [HttpDelete("{id}/remove-teacher/{teacherId}")]
    public async Task<IActionResult> RemoveTeacher(string id, string teacherId)
    {
        var command = new RemoveTeacherCommand(id, teacherId);
        await _messageBus.SendAsync<RemoveTeacherCommand>(command);
        return NoContent();
    }
}

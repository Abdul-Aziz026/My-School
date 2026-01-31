using Application.Common.Interfaces.Publisher;
using Application.Features.SchoolClassManagement.DTOs;
using Application.Features.SchoolClassManagement.Queries.GetTeachers;
using Application.Features.SchoolClassManagement.Queries.GetTeacherClasses;
using Microsoft.AspNetCore.Mvc;
using Application.Features.Common.Models;
using Application.Features.SchoolClassManagement.TeacherManagement.Commands.AssignTeacherToClass;
using Application.Features.SchoolClassManagement.TeacherManagement.Commands.CreateTeacher;
using Application.Features.SchoolClassManagement.TeacherManagement.Commands.DeleteTeacher;
using Application.Features.SchoolClassManagement.TeacherManagement.Commands.UnassignTeacherFromClass;
using Application.Features.SchoolClassManagement.TeacherManagement.Commands.UpdateTeacher;
using Application.Features.SchoolClassManagement.TeacherManagement.Queries.GetTeacherById;
using Application.Features.SchoolClassManagement.TeacherManagement.Queries.GetTeacherSubjects;
using Application.Features.SchoolClassManagement.TeacherManagement.DTOs;
using Application.Features.SchoolClassManagement.ClassManagement.DTOs;
using Application.Features.SchoolClassManagement.SubjectManagement.DTOs;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeachersController : ControllerBase
{
    private readonly IMessageBus _messageBus;

    public TeachersController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    /// <summary>
    /// Creates a new teacher record
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TeacherResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTeacher([FromBody] CreateTeacherDto requestDto)
    {
        if (!ModelState.IsValid)
        {
            var errorList = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(m => m.ErrorMessage)
                .ToList();
            return BadRequest(new { Errors = errorList });
        }

        var command = requestDto.ToCreateTeacherCommand();
        var responseId = await _messageBus.SendAsync<CreateTeacherCommand, string>(command);

        return CreatedAtAction(
            nameof(GetTeacherById),
            new { id = responseId },
            new { Id = responseId }
        );
    }

    /// <summary>
    /// Gets all teachers with optional pagination and filtering
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<TeacherResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTeachers([FromQuery] GetTeachersQueryDto queryDto)
    {
        var query = queryDto.ToGetTeacherQuery();

        var result = await _messageBus.SendAsync<GetTeachersQuery, PagedResult<TeacherResponseDto>>(query);
        return Ok(result);
    }

    /// <summary>
    /// Gets a specific teacher by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TeacherResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTeacherById(string id)
    {
        var query = new GetTeacherByIdQuery(id);
        var result = await _messageBus.SendAsync<GetTeacherByIdQuery, TeacherResponseDto>(query);
        return Ok(result);
    }

    /// <summary>
    /// Updates an existing teacher
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTeacher(string id, [FromBody] UpdateTeacherDto requestDto)
    {
        if (!ModelState.IsValid)
        {
            var errorList = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(m => m.ErrorMessage)
                .ToList();
            return BadRequest(new { Errors = errorList });
        }

        var command = requestDto.ToUpdateTeacherCommand(id);
        await _messageBus.SendAsync<UpdateTeacherCommand>(command);
        return NoContent();
    }

    /// <summary>
    /// Deletes a teacher (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTeacher(string id)
    {
        var command = new DeleteTeacherCommand(id);
        await _messageBus.SendAsync<DeleteTeacherCommand>(command);
        return NoContent();
    }

    /// <summary>
    /// Gets all classes taught by a teacher
    /// </summary>
    [HttpGet("{teacherId}/classes")]
    [ProducesResponseType(typeof(List<ClassResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTeacherClasses(string teacherId)
    {
        var query = new GetTeacherClassesQuery(teacherId);
        var result = await _messageBus.SendAsync<GetTeacherClassesQuery, List<ClassResponseDto>>(query);
        return Ok(result);
    }

    /// <summary>
    /// Gets all subjects a teacher is qualified to teach
    /// </summary>
    [HttpGet("{teacherId}/subjects")]
    [ProducesResponseType(typeof(List<SubjectResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTeacherSubjects(string teacherId)
    {
        var query = new GetTeacherSubjectsQuery(teacherId);
        var result = await _messageBus.SendAsync<GetTeacherSubjectsQuery, List<SubjectResponseDto>>(query);
        return Ok(result);
    }

    /// <summary>
    /// Assigns a teacher to a class
    /// </summary>
    [HttpPost("assign-class")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignTeacherToClass(AssignTeacherToClassDto request)
    {
        if (!ModelState.IsValid)
        {
            var errorList = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(m => m.ErrorMessage)
                .ToList();
            return BadRequest(new { Errors = errorList });
        }

        var command = request.ToAssignTeacherToClassCommand();

        var assignedId = await _messageBus.SendAsync<AssignTeacherToClassCommand, string>(command);
        return Ok(assignedId);
    }

    /// <summary>
    /// Unassigns a teacher from a class
    /// </summary>
    [HttpPost("unassign-class")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnassignTeacherFromClass(UnassignTeacherFromClassDto request)
    {
        if (!ModelState.IsValid)
        {
            var errorList = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(m => m.ErrorMessage)
                .ToList();
            return BadRequest(new { Errors = errorList });
        }

        var command = request.ToUnassignTeacherFromClassCommand();
        await _messageBus.SendAsync<UnassignTeacherFromClassCommand>(command);
        return NoContent();
    }
}

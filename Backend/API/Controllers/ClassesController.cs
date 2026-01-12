using Application.Common.Interfaces.Publisher;
using Application.Features.SchoolClassManagement.Commands.CreateClass;
using Application.Features.SchoolClassManagement.Commands.DeleteClass;
using Application.Features.SchoolClassManagement.Commands.EnrollStudent;
using Application.Features.SchoolClassManagement.Commands.TransferStudent;
using Application.Features.SchoolClassManagement.Commands.UnenrollStudent;
using Application.Features.SchoolClassManagement.Commands.UpdateClass;
using Application.Features.SchoolClassManagement.DTOs;
using Application.Features.SchoolClassManagement.Queries.GetClassById;
using Application.Features.SchoolClassManagement.Queries.GetClassStudents;
using Application.Features.SchoolClassManagement.Queries.GetStudentClasses;
using Application.Features.SchoolClassManagement.Queries.GetTeacherClasses;
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

    /// <summary>
    /// Gets all classes taught by a teacher
    /// </summary>
    [HttpGet("teacher/{teacherId}")]
    public async Task<IActionResult> GetTeacherClasses(string teacherId)
    {
        var query = new GetTeacherClassesQuery(teacherId);
        var result = await _messageBus.SendAsync<GetTeacherClassesQuery, List<ClassResponseDto>> (query);
        return Ok(result);
    }


    /// <summary>
    /// Enrolls a student in a class
    /// </summary>
    /// <returns>Enrollment ID</returns>
    [HttpPost("enroll")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EnrollStudent([FromBody] UnEnrollStudentRequestDto request)
    {
        var command = new EnrollStudentCommand(request.StudentId, request.ClassId);

        // Command returns enrollment ID
        var enrollmentId = await _messageBus.SendAsync<EnrollStudentCommand, string>(command);

        // Return 201 with enrollment ID
        // Client can query student's classes to see full details
        return CreatedAtAction(
            nameof(GetStudentClasses),
            new { enrollmentId }
        );
    }

    /// <summary>
    /// Unenrolls a student from a class
    /// </summary>
    [HttpPost("unenroll")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnenrollStudent([FromBody] UnEnrollStudentRequestDto request)
    {
        var command = request.ToUnenrollStudentCommand();
        await _messageBus.SendAsync<UnenrollStudentCommand>(command);
        return NoContent();
    }

    /// <summary>
    /// Transfers a student from one class to another
    /// </summary>
    /// <returns>New enrollment ID</returns>
    [HttpPost("transfer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> TransferStudent([FromBody] TransferStudentRequestDto request)
    {
        var command = request.ToTransferStudentCommand();

        // Command returns new enrollment ID
        var newEnrollmentId = await _messageBus.SendAsync<TransferStudentCommand, string>(command);

        // Return 200 OK with new enrollment ID
        return Ok(new { Id = newEnrollmentId });
    }

    /// <summary>
    /// Gets all classes a student is enrolled in
    /// </summary>
    [HttpGet("student/{studentId}")]
    [ProducesResponseType(typeof(List<ClassResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudentClasses(string studentId)
    {
        var query = new GetStudentClassesQuery(studentId);
        var result = await _messageBus.SendAsync<GetStudentClassesQuery, List<ClassResponseDto>>(query);
        return Ok(result);
    }
}


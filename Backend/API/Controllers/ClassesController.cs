using Application.Common.Interfaces.Publisher;
using Application.Features.SchoolClassManagement.Commands.CreateClass;
using Application.Features.SchoolClassManagement.Commands.DeleteClass;
using Application.Features.SchoolClassManagement.Commands.UpdateClass;
using Application.Features.SchoolClassManagement.DTOs;
using Application.Features.SchoolClassManagement.Queries.GetClassById;
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

        if (result == null)
            return NotFound(new { message = $"Class with ID {id} not found" });

        return Ok(result);
    }

    /// <summary>
    /// Updates an existing class
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateClass(string id, [FromBody] UpdateClassDto dto)
    {
        var command = dto.ToUpdateClassCommand();

        // Command returns Unit (void)
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
    [ProducesResponseType(typeof(ClassWithStudentsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetClassStudents(string id)
    {
        var query = new GetClassStudentsQuery(id);
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound(new { message = $"Class with ID {id} not found" });

        return Ok(result);
    }

    /// <summary>
    /// Gets all classes taught by a teacher
    /// </summary>
    [HttpGet("teacher/{teacherId}")]
    [ProducesResponseType(typeof(List<ClassDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTeacherClasses(string teacherId)
    {
        var query = new GetTeacherClassesQuery(teacherId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}

[ApiController]
[Route("api/[controller]")]
public class EnrollmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EnrollmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Enrolls a student in a class
    /// </summary>
    /// <returns>Enrollment ID</returns>
    [HttpPost("enroll")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EnrollStudent([FromBody] EnrollStudentRequest request)
    {
        var command = new EnrollStudentCommand(request.StudentId, request.ClassId);

        // Command returns enrollment ID
        var enrollmentId = await _mediator.Send(command);

        // Return 201 with enrollment ID
        // Client can query student's classes to see full details
        return CreatedAtAction(
            nameof(GetStudentClasses),
            new { studentId = request.StudentId },
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
    public async Task<IActionResult> UnenrollStudent([FromBody] UnenrollStudentRequest request)
    {
        var command = new UnenrollStudentCommand(request.StudentId, request.ClassId);
        await _mediator.Send(command);

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
    public async Task<IActionResult> TransferStudent([FromBody] TransferStudentRequest request)
    {
        var command = new TransferStudentCommand(
            request.StudentId,
            request.FromClassId,
            request.ToClassId
        );

        // Command returns new enrollment ID
        var newEnrollmentId = await _mediator.Send(command);

        // Return 200 OK with new enrollment ID
        return Ok(new { enrollmentId = newEnrollmentId });
    }

    /// <summary>
    /// Gets all classes a student is enrolled in
    /// </summary>
    [HttpGet("student/{studentId}")]
    [ProducesResponseType(typeof(List<StudentClassDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudentClasses(string studentId)
    {
        var query = new GetStudentClassesQuery(studentId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}

// Request DTOs remain the same
public record EnrollStudentRequest(string StudentId, string ClassId);
public record UnenrollStudentRequest(string StudentId, string ClassId);
public record TransferStudentRequest(string StudentId, string FromClassId, string ToClassId);

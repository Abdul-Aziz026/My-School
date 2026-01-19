using Application.Common.Interfaces.Publisher;
using Application.Features.SchoolClassManagement.Commands.EnrollStudent;
using Application.Features.SchoolClassManagement.Commands.UnenrollStudent;
using Application.Features.SchoolClassManagement.DTOs;
using Application.Features.SchoolClassManagement.Queries.GetStudentClasses;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : Controller
{

    private readonly IMessageBus _messageBus;
    public StudentsController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    /// <summary>
    /// Enrolls a student in a class
    /// </summary>
    /// <returns>Enrollment ID</returns>
    [HttpPost("enroll")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EnrollStudent([FromBody] EnrollStudentRequestDto request)
    {
        var command = request.ToEnrollStudentCommand();

        // Command returns enrollment ID
        var enrolledResponse = await _messageBus.SendAsync<EnrollStudentCommand, EnrollStudentResponseDto>(command);

        if (!enrolledResponse.Success)
        {
            return BadRequest(new { Message = enrolledResponse.Message });
        }
        // Return 201 with enrollment ID
        // Client can query student's classes to see full details
        return CreatedAtAction("Enrolled", enrolledResponse);
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

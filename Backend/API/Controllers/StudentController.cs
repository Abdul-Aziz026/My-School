using Application.Common.Interfaces.Publisher;
using Application.Features.Common.Models;
using Application.Features.SchoolClassManagement.ClassManagement.DTOs;
using Application.Features.SchoolClassManagement.StudentManagement.Commands.CreateStudent;
using Application.Features.SchoolClassManagement.StudentManagement.Commands.DeleteStudent;
using Application.Features.SchoolClassManagement.StudentManagement.Commands.EnrollStudent;
using Application.Features.SchoolClassManagement.StudentManagement.Commands.SubmitExam;
using Application.Features.SchoolClassManagement.StudentManagement.Commands.UnenrollStudent;
using Application.Features.SchoolClassManagement.StudentManagement.Commands.UpdateStudent;
using Application.Features.SchoolClassManagement.StudentManagement.DTOs;
using Application.Features.SchoolClassManagement.StudentManagement.Queries.GetExamResults;
using Application.Features.SchoolClassManagement.StudentManagement.Queries.GetStudentById;
using Application.Features.SchoolClassManagement.StudentManagement.Queries.GetStudentClasses;
using Application.Features.SchoolClassManagement.StudentManagement.Queries.GetStudentResult;
using Application.Features.SchoolClassManagement.StudentManagement.Queries.GetStudents;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentController : Controller
{

    private readonly IMessageBus _messageBus;
    public StudentController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    /// <summary>
    /// Creates a new student record
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateStudentResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateStudent([FromBody] CreateStudentDto requestDto)
    {
        if (!ModelState.IsValid)
        {
            var errorList = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(m => m.ErrorMessage)
                .ToList();
            return BadRequest(new { Errors = errorList });
        }

        var command = requestDto.ToCreateStudentCommand();
        var response = await _messageBus.SendAsync<CreateStudentCommand, CreateStudentResponseDto>(command);

        return CreatedAtAction(
            nameof(GetStudentById),
            new { id = response.StudentId },
            new { StudentNumer = response.StudentNumber }
        );
    }

    /// <summary>
    /// Gets all students with optional pagination and filtering
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<StudentResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudents([FromQuery] GetStudentsQueryDto queryDto)
    {
        var query = new GetStudentsQuery
        {
            Page = queryDto.Page ?? 1,
            PageSize = queryDto.PageSize ?? 10,
            Search = queryDto.Search,
            Grade = queryDto.GradeLevel,
            ClassId = queryDto.ClassId,
            Status = queryDto.Status
        };

        var result = await _messageBus.SendAsync<GetStudentsQuery, PagedResult<StudentResponseDto>>(query);
        return Ok(result);
    }

    /// <summary>
    /// Gets a specific student by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(StudentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudentById(string id)
    {
        var query = new GetStudentByIdQuery( id);
        var result = await _messageBus.SendAsync<GetStudentByIdQuery, StudentResponseDto>(query);
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
        return CreatedAtAction(null, enrolledResponse);
    }

    /// <summary>
    /// Updates an existing student
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStudent(string id, [FromBody] UpdateStudentDto requestDto)
    {
        if (!ModelState.IsValid)
        {
            var errorList = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(m => m.ErrorMessage)
                .ToList();
            return BadRequest(new { Errors = errorList });
        }

        var command = requestDto.ToUpdateStudentCommand(id);

        await _messageBus.SendAsync<UpdateStudentCommand>(command);
        return NoContent();
    }
    
    /// <summary>
    /// Deletes a student (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteStudent(string id)
    {
        var command = new DeleteStudentCommand(id);
        await _messageBus.SendAsync<DeleteStudentCommand>(command);
        return NoContent();
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

    [HttpPost("submit")]
    public async Task<IActionResult> SubmitExam(SubmitExamDto studentAnswer)
    {
        var command = studentAnswer.ToSubmitExamCommand();
        var result = await _messageBus.SendAsync<SubmitExamCommand, ExamResultDto>(command);
        return Ok(result);
    }

    [HttpGet("result/{examId}/{studentId}")]
    public async Task<IActionResult> GetResult(string examId, string studentId)
    {
        var query = new GetStudentResultQuery
        {
            ExamId = examId,
            StudentId = studentId
        };
        var result = await _messageBus.SendAsync<GetStudentResultQuery, ExamResultDto>(query);
        return Ok(result);
    }

    [HttpGet("results/{examId}")]
    public async Task<IActionResult> GetExamResults(string examId)
    {
        var query = new GetExamResultsQuery
        {
            ExamId = examId
        };
        var results = await _messageBus.SendAsync<GetExamResultsQuery, List<ExamResultDto>>(query);
        return Ok(results);
    }
}

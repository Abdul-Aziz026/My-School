using Application.Common.Interfaces.Publisher;
using Application.Features.SchoolClassManagement.Commands.CreateTeacher;
using Application.Features.SchoolClassManagement.Commands.UpdateTeacher;
using Application.Features.SchoolClassManagement.Commands.DeleteTeacher;
using Application.Features.SchoolClassManagement.Commands.AssignTeacherToClass;
using Application.Features.SchoolClassManagement.Commands.UnassignTeacherFromClass;
using Application.Features.SchoolClassManagement.DTOs;
using Application.Features.SchoolClassManagement.Queries.GetTeachers;
using Application.Features.SchoolClassManagement.Queries.GetTeacherById;
using Application.Features.SchoolClassManagement.Queries.GetTeacherClasses;
using Application.Features.SchoolClassManagement.Queries.GetTeacherSubjects;
using Microsoft.AspNetCore.Mvc;

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
    [ProducesResponseType(typeof(CreateTeacherResponseDto), StatusCodes.Status201Created)]
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
        var query = new GetTeachersQuery
        {
            Page = queryDto.Page ?? 1,
            PageSize = queryDto.PageSize ?? 10,
            Search = queryDto.Search,
            Department = queryDto.Department,
            SubjectId = queryDto.SubjectId,
            IsActive = queryDto.IsActive
        };

        var result = await _messageBus.SendAsync<GetTeachersQuery, PagedResult<TeacherResponseDto>>(query);
        return Ok(result);
    }

    /// <summary>
    /// Gets a specific teacher by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TeacherDetailResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTeacherById(string id)
    {
        var query = new GetTeacherByIdQuery { Id = id };
        var result = await _messageBus.SendAsync<GetTeacherByIdQuery, TeacherDetailResponseDto>(query);

        if (result == null)
        {
            return NotFound(new { Message = $"Teacher with ID '{id}' not found." });
        }

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

        var command = new UpdateTeacherCommand
        {
            Id = id,
            FirstName = requestDto.FirstName,
            LastName = requestDto.LastName,
            Email = requestDto.Email,
            PhoneNumber = requestDto.PhoneNumber,
            Department = requestDto.Department,
            Specialization = requestDto.Specialization,
            HireDate = requestDto.HireDate,
            Qualifications = requestDto.Qualifications,
            IsActive = requestDto.IsActive
        };

        await _messageBus.SendAsync<UpdateTeacherCommand, bool>(command);
        return NoContent();
    }

    /// <summary>
    /// Deletes a teacher (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTeacher(string id)
    {
        var command = new DeleteTeacherCommand { Id = id };
        await _messageBus.SendAsync<DeleteTeacherCommand, bool>(command);
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
        var query = new GetTeacherSubjectsQuery { TeacherId = teacherId };
        var result = await _messageBus.SendAsync<GetTeacherSubjectsQuery, List<SubjectResponseDto>>(query);
        return Ok(result);
    }

    /// <summary>
    /// Assigns a teacher to a class
    /// </summary>
    [HttpPost("assign-class")]
    [ProducesResponseType(typeof(AssignTeacherResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignTeacherToClass([FromBody] AssignTeacherToClassDto request)
    {
        if (!ModelState.IsValid)
        {
            var errorList = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(m => m.ErrorMessage)
                .ToList();
            return BadRequest(new { Errors = errorList });
        }

        var command = new AssignTeacherToClassCommand
        {
            TeacherId = request.TeacherId,
            ClassId = request.ClassId,
            IsPrimaryTeacher = request.IsPrimaryTeacher
        };

        var response = await _messageBus.SendAsync<AssignTeacherToClassCommand, AssignTeacherResponseDto>(command);

        if (!response.Success)
        {
            return BadRequest(new { Message = response.Message });
        }

        return Ok(response);
    }

    /// <summary>
    /// Unassigns a teacher from a class
    /// </summary>
    [HttpPost("unassign-class")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnassignTeacherFromClass([FromBody] UnassignTeacherFromClassDto request)
    {
        if (!ModelState.IsValid)
        {
            var errorList = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(m => m.ErrorMessage)
                .ToList();
            return BadRequest(new { Errors = errorList });
        }

        var command = new UnassignTeacherFromClassCommand
        {
            TeacherId = request.TeacherId,
            ClassId = request.ClassId
        };

        await _messageBus.SendAsync<UnassignTeacherFromClassCommand>(command);
        return NoContent();
    }
}

// ============================================================================
// DTOs (place these in your Application layer DTOs folder)
// ============================================================================

/// <summary>
/// DTO for creating a teacher
/// </summary>
public class CreateTeacherDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Department { get; set; }
    public string? Specialization { get; set; }
    public DateTime? HireDate { get; set; }
    public string? Qualifications { get; set; }
    public List<string> SubjectIds { get; set; } = new();

    public CreateTeacherCommand ToCreateTeacherCommand() => new CreateTeacherCommand
    {
        FirstName = FirstName,
        LastName = LastName,
        Email = Email,
        PhoneNumber = PhoneNumber,
        Department = Department,
        Specialization = Specialization,
        HireDate = HireDate ?? DateTime.UtcNow,
        Qualifications = Qualifications,
        SubjectIds = SubjectIds
    };
}

/// <summary>
/// DTO for updating a teacher
/// </summary>
public class UpdateTeacherDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Department { get; set; }
    public string? Specialization { get; set; }
    public DateTime? HireDate { get; set; }
    public string? Qualifications { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Query parameters for getting teachers
/// </summary>
public class GetTeachersQueryDto
{
    /// <summary>
    /// Page number (default: 1)
    /// </summary>
    public int? Page { get; set; }

    /// <summary>
    /// Page size (default: 10)
    /// </summary>
    public int? PageSize { get; set; }

    /// <summary>
    /// Search term for teacher name or email
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// Filter by department
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// Filter by subject ID
    /// </summary>
    public string? SubjectId { get; set; }

    /// <summary>
    /// Filter by active status
    /// </summary>
    public bool? IsActive { get; set; }
}

/// <summary>
/// Detailed response DTO for individual teacher view
/// </summary>
public class TeacherDetailResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Department { get; set; }
    public string? Specialization { get; set; }
    public DateTime HireDate { get; set; }
    public int YearsOfService => DateTime.Now.Year - HireDate.Year;
    public string? Qualifications { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<ClassResponseDto> Classes { get; set; } = new();
    public List<SubjectResponseDto> Subjects { get; set; } = new();
}

/// <summary>
/// Response DTO after creating a teacher
/// </summary>
public class CreateTeacherResponseDto
{
    public string Id { get; set; } = string.Empty;
}

/// <summary>
/// DTO for assigning a teacher to a class
/// </summary>
public class AssignTeacherToClassDto
{
    public string TeacherId { get; set; } = string.Empty;
    public string ClassId { get; set; } = string.Empty;
    public bool IsPrimaryTeacher { get; set; } = true;
}

/// <summary>
/// DTO for unassigning a teacher from a class
/// </summary>
public class UnassignTeacherFromClassDto
{
    public string TeacherId { get; set; } = string.Empty;
    public string ClassId { get; set; } = string.Empty;
}

/// <summary>
/// Response DTO for teacher assignment
/// </summary>
public class AssignTeacherResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? AssignmentId { get; set; }
}

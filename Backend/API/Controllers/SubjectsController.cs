using Application.Common.Interfaces.Publisher;
using Application.Features.Common.Models;
using Application.Features.SchoolClassManagement.ClassManagement.DTOs;
using Application.Features.SchoolClassManagement.Queries.GetSubjectById;
using Application.Features.SchoolClassManagement.Queries.GetSubjectClasses;
using Application.Features.SchoolClassManagement.Queries.GetSubjects;
using Application.Features.SchoolClassManagement.SubjectManagement.Commands.CreateSubject;
using Application.Features.SchoolClassManagement.SubjectManagement.Commands.DeleteSubject;
using Application.Features.SchoolClassManagement.SubjectManagement.Commands.UpdateSubject;
using Application.Features.SchoolClassManagement.SubjectManagement.DTOs;
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

    /// <summary>
    /// Gets all subjects with optional pagination and filtering
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<SubjectResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSubjects([FromQuery] GetSubjectsQueryDto queryDto)
    {
        var query = queryDto.ToGetSubjectsQuery();

        var result = await _messageBus.SendAsync<GetSubjectsQuery, PagedResult<SubjectResponseDto>>(query);
        return Ok(result);
    }

    /// <summary>
    /// Gets a specific subject by ID
    /// </summary>
    /// <param name="id">Subject ID</param>
    /// <returns>Subject details</returns>
    /// <response code="200">Returns the subject</response>
    /// <response code="404">Subject not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SubjectResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSubjectById(string id)
    {
        var query = new GetSubjectByIdQuery(id);
        var result = await _messageBus.SendAsync<GetSubjectByIdQuery, SubjectResponseDto>(query);
        return Ok(result);
    }

    /// <summary>
    /// Updates an existing subject
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSubject(string id, [FromBody] UpdateSubjectDto requestDto)
    {
        if (!ModelState.IsValid)
        {
            var errorList = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(m => m.ErrorMessage)
                .ToList();
            return BadRequest(new { Errors = errorList });
        }

        var command = requestDto.ToUpdateSubjectCommand(id);
        await _messageBus.SendAsync<UpdateSubjectCommand>(command);
        return NoContent();
    }

    /// <summary>
    /// Deletes a subject (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSubject(string id)
    {
        var command = new DeleteSubjectCommand(id);
        await _messageBus.SendAsync<DeleteSubjectCommand>(command);
        return NoContent();
    }

    /// <summary>
    /// Gets all classes that teach this subject
    /// </summary>
    [HttpGet("{id}/classes")]
    [ProducesResponseType(typeof(List<ClassResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSubjectClasses(string id)
    {
        var query = new GetSubjectClassesQuery { SubjectId = id };
        var result = await _messageBus.SendAsync<GetSubjectClassesQuery, List<ClassResponseDto>>(query);
        return Ok(result);
    }
}

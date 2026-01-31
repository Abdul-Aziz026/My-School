using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Domain.Entities;
using Application.Features.SchoolClassManagement.TimeTableManagement.Queries.GetTimeTable;
using Application.Common.Interfaces.Publisher;
using Application.Features.SchoolClassManagement.TimeTableManagement.DTOs;
using Application.Features.SchoolClassManagement.TimeTableManagement.Queries.GetTimeTableById;
using Application.Features.SchoolClassManagement.TimeTableManagement.Queries.GetTimeTableByClassId;
using Application.Features.SchoolClassManagement.TimeTableManagement.Queries.GetTimeTableByTeacherId;
using Application.Features.SchoolClassManagement.TimeTableManagement.Queries.GetTimeTableByAcademicYear;
using Application.Features.SchoolClassManagement.TimeTableManagement.Queries.GetTimeTableByClassAndDay;
using Application.Features.SchoolClassManagement.TimeTableManagement.Commands.CreateTimeTable;
using Application.Features.SchoolClassManagement.TimeTableManagement.Commands.UpdateTimeTable;
using Application.Features.SchoolClassManagement.TimeTableManagement.Commands.DeleteTimeTable;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TimetableController : ControllerBase
{
    private readonly IMessageBus _messageBus;

    public TimetableController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    // GET: api/timetable
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetAll()
    {
        var query = new GetTimeTableQuery();
        var timetables = await _messageBus.SendAsync<GetTimeTableQuery, IEnumerable<TimeTableResponseDto>>(query);
        return Ok(timetables);
    }

    // GET: api/timetable/{id}
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetById(string id)
    {
        var query = new GetTimeTableByIdQuery(id);
        var timetables = await _messageBus.SendAsync<GetTimeTableByIdQuery, TimeTableResponseDto>(query);
        return Ok(timetables);
    }

    // GET: api/timetable/class/{classId}
    [HttpGet("class/{classId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetByClass(string classId)
    {
        var query = new GetTimeTableByClassIdQuery(classId);
        var timetables = await _messageBus.SendAsync<GetTimeTableByClassIdQuery, IEnumerable<TimeTableResponseDto>>(query);
        return Ok(timetables);
    }

    // GET: api/timetable/teacher/{teacherId}
    [HttpGet("teacher/{teacherId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetByTeacher(string teacherId)
    {
        var query = new GetTimeTableByTeacherIdQuery(teacherId);
        var timetables = await _messageBus.SendAsync<GetTimeTableByTeacherIdQuery, IEnumerable<TimeTableResponseDto>>(query);
        return Ok(timetables);
    }

    // GET: api/timetable/academic-year/{year}
    [HttpGet("academic-year/{year}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TimeTable>>> GetByAcademicYear(string year)
    {
        var query = new GetTimeTableByAcademicYearQuery(year);
        var timetables = await _messageBus.SendAsync<GetTimeTableByAcademicYearQuery, IEnumerable<TimeTableResponseDto>>(query);
        return Ok(timetables);
    }

    // GET: api/timetable/class/{classId}/day/{dayOfWeek}
    [HttpGet("class/{classId}/day/{dayOfWeek}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetByClassAndDay(string classId, DayOfWeek dayOfWeek)
    {
        var query = new GetTimeTableByClassAndDayQuery(classId, dayOfWeek);
        var timetables = await _messageBus.SendAsync<GetTimeTableByClassAndDayQuery, IEnumerable<TimeTableResponseDto>>(query);
        return Ok(timetables);
    }


    // POST: api/timetable
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<TimeTable>> Create([FromBody] CreateTimeTableDto timeTableDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ModelState);
        }
        var command = timeTableDto.ToCreateTimeTableCommand();
        var Id = await _messageBus.SendAsync<CreateTimeTableCommand, string>(command);
        
        return CreatedAtAction(
            nameof(GetById),
            new { id = Id });
    }

    // PUT: api/timetable/{id}
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<TimeTable>> Update(string id, [FromBody] UpdateTimeTableDto updateTimeTableDto)
    {
        var command = updateTimeTableDto.ToUpdateTimeTableCommand(id);
        var updated = await _messageBus.SendAsync<UpdateTimeTableCommand, TimeTableResponseDto>(command);
        return Ok(updated);
    }

    // DELETE: api/timetable/{id}
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Delete(string id)
    {
        var deleteCommand = new DeleteTimeTableCommand(id);
        await _messageBus.SendAsync<DeleteTimeTableCommand>(deleteCommand);
        return NoContent();
    }
}
using Application.Common.Interfaces.Publisher;
using Application.Features.SchoolClassManagement.AttendanceManagement.Commands.DeleteAttendance;
using Application.Features.SchoolClassManagement.AttendanceManagement.Commands.RecordEntry;
using Application.Features.SchoolClassManagement.AttendanceManagement.DTOs;
using Application.Features.SchoolClassManagement.AttendanceManagement.Queries.ExportAttendanceData;
using Application.Features.SchoolClassManagement.AttendanceManagement.Queries.GetDailyAttendance;
using Application.Features.SchoolClassManagement.AttendanceManagement.Queries.GetStudentAttendanceHistory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AttendanceController : Controller
{
    private readonly IMessageBus _messageBus;
    public AttendanceController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    /// <summary>
    /// Record Entry time
    /// </summary>
    [HttpPost("entry")]
    public async Task<IActionResult> RecordEntry([FromBody]RecordEntryDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ModelState);
        }
        var command = dto.ToRecordEntryCommand();
        await _messageBus.SendAsync<RecordEntryCommand>(command);
        return Ok("Student Entry Added Successfully");
    }

    /// <summary>
    /// Record student exit time
    /// </summary>
    [HttpPost("exit")]
    public async Task<IActionResult> RecordExit([FromBody] RecordExitDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ModelState);
        }
        var command = dto.ToRecordExitCommand();
        await _messageBus.SendAsync(command);
        return Ok("Student Exit Recorded Successfully");
    }

    /// <summery>
    /// Delete attendance record by ID
    /// </summery>
    [HttpDelete("{attendanceId}")]
    public async Task<IActionResult> DeleteAttendanceRecord(string attendanceId)
    {
        var command = new DeleteAttendanceCommand()
        {
            AttendanceId = attendanceId
        };
        await _messageBus.SendAsync(command);
        return Ok("Attendance Record Deleted Successfully");
    }

    /// <summary>
    /// Update attendance record
    /// </summary>
    [HttpPut("update/{attendanceId}")]
    public async Task<IActionResult> UpdateAttendanceRecord([FromRoute] string attendanceId, [FromBody] UpdateAttendanceDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ModelState);
        }

        var command = dto.ToUpdateAttendanceCommand(attendanceId);
        await _messageBus.SendAsync(command);
        return Ok("Attendance Record Updated Successfully");
    }

    /// <summary>
    /// Get daily attendance for a class
    /// </summary>
    [HttpGet("daily/{classId}")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> GetDailyAttendance([FromRoute]string classId, [FromQuery]DateTime date)
    {
        var query = new GetDailyAttendanceQuery()
        {
            ClassId = classId,
            Date = date
        };
        var result = await _messageBus.SendAsync<GetDailyAttendanceQuery, List<AttendanceResponseDto>>(query);
        return Ok(result);
    }

    /// <summary>
    /// Get attendance history for a student
    /// </summary>
    [HttpGet("student/{studentId}/history")]
    [Authorize(Roles = "Admin,Teacher,Student")]
    public async Task<IActionResult> GetStudentAttendanceHistory([FromRoute]string studentId,
                                                                 [FromQuery]DateTime? startDate = null,
                                                                 [FromQuery]DateTime? endDate = null)
    {
        var query = new GetStudentAttendanceHistoryQuery()
        {
            StudentId = studentId,
            StartDate = startDate,
            EndDate = endDate
        };
        var result = await _messageBus.SendAsync<GetStudentAttendanceHistoryQuery, List<AttendanceResponseDto>>(query);
        return Ok(result);
    }

    /// <summary>
    /// Export attendance data in Excel/PDF format
    /// </summary>
    /// Parameters: format (excel/pdf), startDate, endDate
    [HttpGet("export")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> ExportAttendanceData([FromQuery] string format, 
                                                          [FromQuery] DateTime startDate, 
                                                          [FromQuery] DateTime endDate,
                                                          [FromQuery] string? classId = null)
    {
        var query = new ExportAttendanceDataQuery()
        {
            Format = format,
            StartDate = startDate,
            EndDate = endDate,
            ClassId = classId
        };
        var fileResult = await _messageBus.SendAsync<ExportAttendanceDataQuery, AttendanceFileResultDto>(query);
        return Ok(fileResult);
    }
}


using Application.Features.SchoolClassManagement.AttendanceManagement.Commands.UpdateAttendance;

namespace Application.Features.SchoolClassManagement.AttendanceManagement.DTOs;

public class UpdateAttendanceDto
{
    public DateTime? EntryTime { get; set; }
    public DateTime? ExitTime { get; set; }
    public string? Status { get; set; }
    public DateTime? Date { get; set; }

    public UpdateAttendanceCommand ToUpdateAttendanceCommand(string attendanceId)
    {
        return new UpdateAttendanceCommand()
        {
            AttendanceId = attendanceId,
            EntryTime = this.EntryTime,
            ExitTime = this.ExitTime,
            Date = this.Date
        };
    }
}

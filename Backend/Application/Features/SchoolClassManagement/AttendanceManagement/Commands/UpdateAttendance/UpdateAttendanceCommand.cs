
using MediatR;

namespace Application.Features.SchoolClassManagement.AttendanceManagement.Commands.UpdateAttendance;

public class UpdateAttendanceCommand : IRequest
{
    public string AttendanceId { get; set; }
    public DateTime? EntryTime { get; set; }
    public DateTime? ExitTime{ get; set; }
    public DateTime? Date { get; set; }
}

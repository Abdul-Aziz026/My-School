using MediatR;

namespace Application.Features.SchoolClassManagement.AttendanceManagement.Commands.DeleteAttendance;

public class DeleteAttendanceCommand : IRequest
{
    public string AttendanceId { get; set; } = string.Empty;
}

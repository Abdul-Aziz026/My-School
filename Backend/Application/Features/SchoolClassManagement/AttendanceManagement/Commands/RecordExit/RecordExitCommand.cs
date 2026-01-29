
using MediatR;

namespace Application.Features.SchoolClassManagement.AttendanceManagement.Commands.RecordExit;

public class RecordExitCommand : IRequest
{
    public string StudentId { get; set; }
    public DateTime ExitTime { get; set; }
    public string ClassId { get; set; }
    public DateTime Date { get; set; }
}

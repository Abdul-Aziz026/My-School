
using MediatR;

namespace Application.Features.SchoolClassManagement.AttendanceManagement.Commands.RecordEntry;

public class RecordEntryCommand : IRequest
{
    public string StudentId { get; set; } = string.Empty;
    public string ClassId { get; set; } = string.Empty;
    public DateTime EntryTime { get; set; }
}

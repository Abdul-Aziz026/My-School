
using Application.Features.SchoolClassManagement.TimeTableManagement.DTOs;
using MediatR;

namespace Application.Features.SchoolClassManagement.TimeTableManagement.Commands.CreateTimeTable;

public class CreateTimeTableCommand : IRequest<string>
{
    public string TimeSlotId { get; set; } = string.Empty;
    public string SubjectId { get; set; } = string.Empty;
    public string ClassId { get; set; } = string.Empty;
    public string TeacherId { get; set; } = string.Empty;
    public DayOfWeek DayOfWeek { get; set; }
    public string RoomNo { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
}

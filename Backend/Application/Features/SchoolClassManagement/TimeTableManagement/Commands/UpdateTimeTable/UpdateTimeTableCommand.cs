
using Application.Features.SchoolClassManagement.TimeTableManagement.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Features.SchoolClassManagement.TimeTableManagement.Commands.UpdateTimeTable;

public class UpdateTimeTableCommand : IRequest<TimeTableResponseDto>
{
    public string Id { get; set; }
    public string TimeSlotId { get; set; }
    public string SubjectId { get; set; }
    public string TeacherId { get; set; }
    public string ClassId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public string RoomNumber { get; set; }
    public string AcademicYear { get; set; }
}

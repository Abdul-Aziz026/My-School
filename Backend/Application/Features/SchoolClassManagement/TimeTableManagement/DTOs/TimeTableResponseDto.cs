
namespace Application.Features.SchoolClassManagement.TimeTableManagement.DTOs;

public class TimeTableResponseDto
{
    public string Id { get; set; } = string.Empty;

    // Flattened Slot Info
    public string TimeSlotId { get; set; } = string.Empty;
    public string SlotName { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;

    // Entity Details (Human Readable)
    public string SubjectId { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string TeacherId { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public string ClassId { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;

    // Enums or Metadata
    public DayOfWeek DayOfWeek { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
}

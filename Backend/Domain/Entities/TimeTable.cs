
namespace Domain.Entities;

public class TimeTable : BaseEntity
{
    public string TimeSlotId { get; set; } = string.Empty;
    public string SubjectId { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string TeacherId { get; set; } = string.Empty;   
    public string TeacherName { get; set; } = string.Empty;
    public string ClassId { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public DayOfWeek DayOfWeek { get; set; }
    public RoomNumber RoomNumber { get; set; }
    public string AcademicYear { get; set; } = string.Empty;
}

public enum RoomNumber
{
    Room101,
    Room102,
    Room103,
    Room104,
    Room105,
    Room201,
    Room202,
    Room203,
    Room204,
    Room205
}

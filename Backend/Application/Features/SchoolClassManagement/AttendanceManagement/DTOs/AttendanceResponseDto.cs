namespace Application.Features.SchoolClassManagement.AttendanceManagement.DTOs;

public class AttendanceResponseDto
{
    public string AttendanceId { get; set; }
    public string StudentId { get; set; }
    public string StudentName { get; set; }
    public string ClassId { get; set; }
    public string ClassName { get; set; }
    public DateTime Date { get; set; }
    public DateTime EntryTime { get; set; }
    public DateTime ExitTime { get; set; }
    public bool IsPresent { get; set; }
}
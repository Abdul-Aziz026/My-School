namespace Application.Features.SchoolClassManagement.ExamManagement.DTOs;

public class ExamDto
{
    public string Id { get; set; } = string.Empty;
    public string ClassId { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string ExamName { get; set; } = string.Empty;
    public string ExamType { get; set; } = string.Empty;
    public DateTime ExamDate { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int Duration { get; set; }
    public int TotalMarks { get; set; }
    public int PassingMarks { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
}
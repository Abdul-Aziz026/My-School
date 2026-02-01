namespace Application.Features.SchoolClassManagement.StudentManagement.DTOs;

public class StudentResultDto
{
    public string StudentId { get; set; } = string.Empty;
    public string ExamId { get; set; } = string.Empty;
    public decimal TotalMarks { get; set; }
    public decimal ObtainedMarks { get; set; }
    public decimal Percentage { get; set; }
    public bool IsPassed { get; set; }
}
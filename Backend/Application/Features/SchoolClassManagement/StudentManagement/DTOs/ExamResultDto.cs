using Domain.Entities;

namespace Application.Features.SchoolClassManagement.StudentManagement.DTOs;

public class ExamResultDto
{
    public string ExamId { get; set; } = string.Empty;
    public string StudentId { get; set; } = string.Empty;
    public decimal TotalMarks { get; set; }
    public decimal ObtainedMarks { get; set; }
    public decimal Percentage { get; set; }
    public bool IsPassed { get; set; }
    public DateTime EvaluatedAt { get; set; }
}

public static class ExamResultDtoExtensions
{
    public static ExamResultDto ToExamResultDto(this ExamResult examResult)
    {
        return new ExamResultDto
        {
            ExamId = examResult.ExamId,
            StudentId = examResult.StudentId,
            TotalMarks = examResult.TotalMarks,
            ObtainedMarks = examResult.ObtainedMarks,
            Percentage = examResult.Percentage,
            IsPassed = examResult.IsPassed,
            EvaluatedAt = examResult.EvaluatedAt,
        };
    }
}
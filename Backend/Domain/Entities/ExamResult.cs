namespace Domain.Entities;

public class ExamResult : BaseEntity
{
    public string ExamId { get; set; } = string.Empty;
    public string StudentId { get; set; } = string.Empty;
    public decimal TotalMarks { get; set; }
    public decimal ObtainedMarks { get; set; }
    public decimal Percentage { get; set; }
    public bool IsPassed { get; set; }
    public DateTime EvaluatedAt { get; set; } = DateTime.UtcNow;
}

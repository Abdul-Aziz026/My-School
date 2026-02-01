namespace Domain.Entities;

public class Exam : BaseEntity
{
    public string ClassId { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string ExamName { get; set; } = string.Empty;
    public string ExamType { get; set; } = string.Empty; // midterm, final, quiz
    public DateTime ExamDate { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int Duration { get; set; } // in minutes
    public int TotalMarks { get; set; }
    public int PassingMarks { get; set; }
    public bool IsPublished { get; set; } = false;

}
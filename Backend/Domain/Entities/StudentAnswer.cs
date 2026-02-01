namespace Domain.Entities;

public class StudentAnswer : BaseEntity
{
    public string ExamId { get; set; } = string.Empty;
    public string StudentId { get; set; } = string.Empty;
    public List<Answer> Answers { get; set; } = new();
    public int TotalMarksObtained { get; set; }
    public DateTime SubmittedAt { get; set; }
    public bool IsGraded { get;set; }
}

public class Answer
{
    public string QuestionId { get; set; } = string.Empty;
    public string StudentAnswer { get; set; } = string.Empty;
    public int MarksObtained { get; set; }
}
namespace Domain.Entities;

public class ExamPaper : BaseEntity
{
    public string ExamId { get; set; } = string.Empty;
    public List<string> QuestionIds { get; set; } = new();
}
namespace Application.Features.SchoolClassManagement.ExamManagement.DTOs;

public class AssignQuestionsDto
{
    public string ExamId { get; set; } = string.Empty;
    public List<string> QuestionIds { get; set; } = new();
}
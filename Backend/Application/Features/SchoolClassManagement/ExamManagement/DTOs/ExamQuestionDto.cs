namespace Application.Features.SchoolClassManagement.ExamManagement.DTOs;

public class ExamQuestionDto
{
    public string QuestionId { get; set; } = string.Empty;
    public string QuestionText { get; set; } = string.Empty;
    public string QuestionType { get; set; } = string.Empty;
    public List<string> Options { get; set; } = new();
    public decimal Marks { get; set; }
    // CorrectAnswer is intentionally excluded from this DTO
    // (students should not see the answer when fetching exam questions)
}
using Domain;

namespace Application.Features.SchoolClassManagement.QuestionManagement.DTOs;

public class QuestionDto
{
    public string Id { get; set; } = string.Empty;
    public string QuestionText { get; set; } = string.Empty;
    public string QuestionType { get; set; } = string.Empty;
    public List<string> Options { get; set; } = new();
    public string CorrectAnswer { get; set; } = string.Empty;
    public string CorrectAnswerText { get; set; } = string.Empty;
    public decimal Marks { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}


public static class QuestionDtoExtensions
{
    public static QuestionDto ToQuestionDto(this Question question)
    {
        return new QuestionDto
        {
            Id = question.Id,
            QuestionText = question.QuestionText,
            QuestionType = question.QuestionType,
            Options = question.Options,
            CorrectAnswer = question.CorrectAnswer,
            CorrectAnswerText = question.CorrectAnswerText,
            Marks = question.Marks,
            SubjectName = question.SubjectName,
            CreatedAt = question.CreatedAt
        };
    }
}
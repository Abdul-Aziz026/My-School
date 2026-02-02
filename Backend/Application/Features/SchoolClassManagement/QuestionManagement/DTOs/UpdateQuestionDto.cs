
using Application.Features.SchoolClassManagement.QuestionManagement.Commands.UpdateQuestion;

namespace Application.Features.SchoolClassManagement.QuestionManagement.DTOs;

public class UpdateQuestionDto
{
    public string Id { get; set; } = string.Empty;  // Required to identify which question to update
    public string QuestionText { get; set; } = string.Empty;
    public string QuestionType { get; set; } = string.Empty;
    public List<string> Options { get; set; } = new();
    public string CorrectAnswer { get; set; } = string.Empty;
    public string CorrectAnswerText { get; set; } = string.Empty;
    public decimal Marks { get; set; }
    public string SubjectName { get; set; } = string.Empty;

    public UpdateQuestionCommand ToUpdateQuestionCommand()
    {
        return new UpdateQuestionCommand
        {
            Id = this.Id,
            QuestionText = this.QuestionText,
            QuestionType = this.QuestionType,
            Options = this.Options,
            CorrectAnswer = this.CorrectAnswer,
            CorrectAnswerText = this.CorrectAnswerText,
            Marks = this.Marks,
            SubjectName = this.SubjectName
        };
    }
}

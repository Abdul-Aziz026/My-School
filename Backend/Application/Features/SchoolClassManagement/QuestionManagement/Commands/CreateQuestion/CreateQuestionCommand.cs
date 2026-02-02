
using Application.Features.SchoolClassManagement.QuestionManagement.DTOs;
using MediatR;

namespace Application.Features.SchoolClassManagement.QuestionManagement.Commands.CreateQuestion;

public class CreateQuestionCommand : IRequest<QuestionDto>
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

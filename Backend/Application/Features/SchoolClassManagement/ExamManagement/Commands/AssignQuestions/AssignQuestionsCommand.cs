
using MediatR;

namespace Application.Features.SchoolClassManagement.ExamManagement.Commands.AssignQuestions;

public class AssignQuestionsCommand : IRequest
{
    public string ExamId { get; set; } = string.Empty;
    public List<string> QuestionIds { get; set; } = new();
}

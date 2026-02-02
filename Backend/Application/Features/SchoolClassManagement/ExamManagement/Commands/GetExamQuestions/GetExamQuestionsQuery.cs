
using Application.Features.SchoolClassManagement.ExamManagement.DTOs;
using MediatR;

namespace Application.Features.SchoolClassManagement.ExamManagement.Commands.GetExamQuestions;

public class GetExamQuestionsQuery : IRequest<List<ExamQuestionDto>>
{
    public string ExamId { get; set; } = string.Empty;
    public GetExamQuestionsQuery(string examId)
    {
        ExamId = examId;
    }
}

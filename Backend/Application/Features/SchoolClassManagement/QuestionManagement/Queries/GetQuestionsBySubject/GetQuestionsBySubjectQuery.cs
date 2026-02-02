
using Application.Features.SchoolClassManagement.QuestionManagement.DTOs;
using MediatR;

namespace Application.Features.SchoolClassManagement.QuestionManagement.Queries.GetQuestionsBySubject;

public class GetQuestionsBySubjectQuery : IRequest<List<QuestionDto>>
{
    public string SubjectName { get; set; } = string.Empty;
}

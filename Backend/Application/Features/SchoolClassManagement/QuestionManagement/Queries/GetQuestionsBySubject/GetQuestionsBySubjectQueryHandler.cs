using Application.Common.Interfaces.Repositories;
using Application.Features.SchoolClassManagement.QuestionManagement.DTOs;
using Domain;
using MediatR;

namespace Application.Features.SchoolClassManagement.QuestionManagement.Queries.GetQuestionsBySubject;

public class GetQuestionsBySubjectQueryHandler : IRequestHandler<GetQuestionsBySubjectQuery, List<QuestionDto>>
{
    private readonly IQuestionRepository _questionRepository;
    public GetQuestionsBySubjectQueryHandler(IQuestionRepository questionRepository)
    {
        _questionRepository = questionRepository;
    }

    public async Task<List<QuestionDto>> Handle(GetQuestionsBySubjectQuery request, CancellationToken cancellationToken)
    {
        var questions = await _questionRepository.GetItemsByConditionAsync<Question>(x => x.SubjectName == request.SubjectName);
        return questions?.Select(q => q.ToQuestionDto()).ToList()!;
    }
}

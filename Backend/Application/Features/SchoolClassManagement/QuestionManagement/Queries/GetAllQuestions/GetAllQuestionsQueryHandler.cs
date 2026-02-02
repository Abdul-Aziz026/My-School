
using Application.Common.Interfaces.Repositories;
using Application.Features.SchoolClassManagement.QuestionManagement.DTOs;
using Domain;
using MediatR;

namespace Application.Features.SchoolClassManagement.QuestionManagement.Queries.GetAllQuestions;

public class GetAllQuestionsQueryHandler : IRequestHandler<GetAllQuestionsQuery, List<QuestionDto>>
{
    private readonly IQuestionRepository _questionRepository;
    public GetAllQuestionsQueryHandler(IQuestionRepository questionRepository)
    {
        _questionRepository = questionRepository;
    }

    public async Task<List<QuestionDto>> Handle(GetAllQuestionsQuery request, CancellationToken cancellationToken)
    {
        var questions = await _questionRepository.GetAllAsync<Question>();
        return questions?.Select(o => o.ToQuestionDto()).ToList()!;
    }
}

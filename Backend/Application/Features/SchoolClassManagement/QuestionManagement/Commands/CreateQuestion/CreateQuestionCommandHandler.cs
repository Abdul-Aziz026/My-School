
using Application.Common.Interfaces.Repositories;
using Application.Features.SchoolClassManagement.QuestionManagement.DTOs;
using Domain;
using MediatR;

namespace Application.Features.SchoolClassManagement.QuestionManagement.Commands.CreateQuestion;

public class CreateQuestionCommandHandler : IRequestHandler<CreateQuestionCommand, QuestionDto>
{
    private readonly IQuestionRepository _questionRepository;
    public CreateQuestionCommandHandler(IQuestionRepository questionRepository)
    {
        _questionRepository = questionRepository;
    }

    public async Task<QuestionDto> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
    {
        var question = new Question()
        {
            QuestionText = request.QuestionText,
            QuestionType = request.QuestionType,
            Options = request.Options,
            CorrectAnswer = request.CorrectAnswer,
            CorrectAnswerText = request.CorrectAnswerText,
            Marks = request.Marks,
            SubjectName = request.SubjectName
        };
        await _questionRepository.AddAsync(question);
        return question.ToQuestionDto();
    }
}

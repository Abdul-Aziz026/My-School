using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Application.Features.SchoolClassManagement.QuestionManagement.DTOs;
using Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.SchoolClassManagement.QuestionManagement.Commands.UpdateQuestion;

public class UpdateQuestionCommandHandler : IRequestHandler<UpdateQuestionCommand, QuestionDto>
{
    private readonly IQuestionRepository _questionRepository;

    public UpdateQuestionCommandHandler(IQuestionRepository questionRepository)
    {
        _questionRepository = questionRepository;
    }
    public async Task<QuestionDto> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
    {
        var question = await _questionRepository.GetByIdAsync<Question>(request.Id);
        if (question == null)
        {
            throw new NotFoundException("Question not found");
        }
        question.QuestionText = request.QuestionText;
        question.QuestionType = request.QuestionType;
        question.Options = request.Options;
        question.CorrectAnswer = request.CorrectAnswer;
        question.CorrectAnswerText = request.CorrectAnswerText;
        question.Marks = request.Marks;
        question.SubjectName = request.SubjectName;
        await _questionRepository.UpdateAsync<Question>(question);
        return question.ToQuestionDto();
    }
}

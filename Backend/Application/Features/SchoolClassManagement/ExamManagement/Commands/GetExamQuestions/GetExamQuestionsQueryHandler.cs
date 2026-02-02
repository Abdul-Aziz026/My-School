using Application.Common.Interfaces.Repositories;
using Application.Features.SchoolClassManagement.ExamManagement.DTOs;
using Domain;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Application.Features.SchoolClassManagement.ExamManagement.Commands.GetExamQuestions;

public class GetExamQuestionsQueryHandler : IRequestHandler<GetExamQuestionsQuery, List<ExamQuestionDto>>
{

    private readonly IExamRepository _examRepository;
    public GetExamQuestionsQueryHandler(IExamRepository examRepository)
    {
        _examRepository = examRepository;
    }

    public async Task<List<ExamQuestionDto>> Handle(GetExamQuestionsQuery request, CancellationToken cancellationToken)
    {
        var examId = request.ExamId;
        var examPaper = await _examRepository.GetByIdAsync<ExamPaper>(examId);
        if (examPaper == null)
        {
            throw new Exception("Exam paper not found");
        }
        Expression<Func<Question, bool>> filter = q => examPaper.QuestionIds.Contains(q.Id);
        var examQuestions = await _examRepository.GetItemsByConditionAsync(filter);
        var examQuestionDtos = examQuestions.Select(q => new ExamQuestionDto
        {
            QuestionId = q.Id,
            QuestionText = q.QuestionText,
            QuestionType = q.QuestionType,
            Options = q.Options,
            Marks = q.Marks
        }).ToList();
        return examQuestionDtos;
    }
}


using Application.Common.Interfaces.Repositories;
using Domain;
using Domain.Entities;
using MediatR;
using System.Linq.Expressions;

namespace Application.Features.SchoolClassManagement.ExamManagement.Commands.AssignQuestions;

public class AssignQuestionsCommandHandler : IRequestHandler<AssignQuestionsCommand>
{
    private readonly IExamRepository _examRepository;
    public AssignQuestionsCommandHandler(IExamRepository examRepository)
    {
        _examRepository = examRepository;
    }

    public async Task Handle(AssignQuestionsCommand command, CancellationToken cancellationToken)
    {
        var examPaper = await _examRepository.GetByIdAsync<ExamPaper>(command.ExamId);

        // validate question ids exist in question bank
        Expression<Func<Question, bool>> filter = q => command.QuestionIds.Contains(q.Id);
        var existingQuestions = await _examRepository.GetItemsByConditionAsync(filter);
        if (existingQuestions is null || existingQuestions.Count != command.QuestionIds.Count)
        {
            throw new Exception("One or more question IDs do not exist in the question bank.");
        }
        if (examPaper == null)
        {
            examPaper = new ExamPaper
            {
                ExamId = command.ExamId,
                QuestionIds = command.QuestionIds
            };
            await _examRepository.AddAsync(examPaper);
        }
        else
        {
            examPaper.QuestionIds = command.QuestionIds;
            await _examRepository.UpdateAsync(examPaper);
        }
    }
}

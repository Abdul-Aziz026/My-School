
using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Application.Features.SchoolClassManagement.StudentManagement.DTOs;
using Domain;
using Domain.Entities;
using MediatR;
using System.Linq.Expressions;

namespace Application.Features.SchoolClassManagement.StudentManagement.Commands.SubmitExam;

public class SubmitExamCommandHandler : IRequestHandler<SubmitExamCommand, ExamResultDto>
{
    private readonly IExamRepository _examRepository;
    private readonly IStudentRepository _studentRepository;
    public SubmitExamCommandHandler(IExamRepository examRepository, IStudentRepository studentRepository)
    {
        _examRepository = examRepository;
        _studentRepository = studentRepository;
    }

    public async Task<ExamResultDto> Handle(SubmitExamCommand request, CancellationToken cancellationToken)
    {
        var exam = await _examRepository.GetByIdAsync<Exam>(request.ExamId);
        if (exam == null)
        {
            throw new NotFoundException("Exam not found");
        }
        if (!exam.IsPublished)
        {
            throw new InvalidOperationException("Exam is not published yet. Students cannot submit.");
        }
        var studentAnswers = new StudentAnswer
        {
            ExamId = request.ExamId,
            StudentId = request.StudentId,
            Answers = request.Answers,
        };
        await _studentRepository.AddAsync(studentAnswers);
        // fetch exam paper and question
        var examPaper = await _examRepository.GetByIdAsync<ExamPaper>(request.ExamId);
        if (examPaper == null)
        {
            throw new NotFoundException("Exam not found");
        }
        Expression<Func<Question, bool>> filter = x => examPaper.QuestionIds.Contains(x.Id);
        var questions = await _examRepository.GetItemsByConditionAsync<Question>(filter);

        // calculate score
        decimal obtainedMarks = 0;
        foreach (var question in questions)
        {
            if (question.QuestionType == "mcq" && request.Answers.TryGetValue(question.Id, out var studentAnswer))
            {
                if (question.CorrectAnswer.Equals(studentAnswer))
                {
                    obtainedMarks += question.Marks; 
                }
            }
        }
        decimal markPercentage = exam.TotalMarks > 0 ?
                Math.Round((obtainedMarks / exam.TotalMarks) * 100, 2) : 0;
        var result = new ExamResult
        {
            ExamId = request.ExamId,
            StudentId = request.StudentId,
            TotalMarks = exam.TotalMarks,
            ObtainedMarks = obtainedMarks,
            Percentage = markPercentage,
            IsPassed = obtainedMarks >= exam.PassingMarks,
            EvaluatedAt = DateTime.UtcNow
        };
        await _examRepository.AddAsync(result);
        return result.ToExamResultDto();
    }
}

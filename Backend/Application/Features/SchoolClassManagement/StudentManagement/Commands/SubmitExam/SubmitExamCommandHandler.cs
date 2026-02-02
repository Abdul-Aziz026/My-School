
using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Application.Features.SchoolClassManagement.StudentManagement.DTOs;
using Domain.Entities;
using MediatR;

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
        var studentAnswer = new StudentAnswer
        {
            ExamId = request.ExamId,
            StudentId = request.StudentId,
            Answers = request.Answers,
        };
        await _studentRepository.AddAsync(studentAnswer);
        // fetch exam paper and question
        /*
         * // 4. Fetch exam paper and questions for evaluation
        var paper = await _examPaperRepository
            .GetByExamIdAsync(dto.ExamId, cancellationToken)
            ?? throw new InvalidOperationException("No exam paper found for this exam.");

        var questions = await _questionRepository
            .GetByIdsAsync(paper.QuestionIds, cancellationToken);

        // 5. Calculate score
        decimal obtainedMarks = 0;

        foreach (var question in questions)
        {
            if (dto.Answers.TryGetValue(question.Id, out var studentAns)
                && studentAns.Equals(question.CorrectAnswer, StringComparison.OrdinalIgnoreCase))
            {
                obtainedMarks += question.Marks;
            }
        }

        decimal percentage = exam.TotalMarks > 0
            ? Math.Round((obtainedMarks / exam.TotalMarks) * 100, 2)
            : 0;

        // 6. Persist result
        var result = new ExamResult
        {
            ExamId         = dto.ExamId,
            StudentId      = dto.StudentId,
            TotalMarks     = exam.TotalMarks,
            ObtainedMarks  = obtainedMarks,
            Percentage     = percentage,
            IsPassed       = obtainedMarks >= exam.PassingMarks
        };

        await _examResultRepository.AddAsync(result, cancellationToken);

        // 7. Return result immediately
        return new ExamResultDto
        {
            Id             = result.Id,
            ExamId         = result.ExamId,
            StudentId      = result.StudentId,
            TotalMarks     = result.TotalMarks,
            ObtainedMarks  = result.ObtainedMarks,
            Percentage     = result.Percentage,
            IsPassed       = result.IsPassed,
            EvaluatedAt    = result.EvaluatedAt
        };
    }
         */
        throw new NotImplementedException();
    }
}

using Application.Common.Interfaces.Repositories;
using Application.Features.SchoolClassManagement.ExamManagement.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Features.SchoolClassManagement.ExamManagement.Queries.GetExamById;

public class GetExamByIdQueryHandler : IRequestHandler<GetExamByIdQuery, ExamDto>
{
    private readonly IExamRepository _examRepository;
    public GetExamByIdQueryHandler(IExamRepository examRepository)
    {
        _examRepository = examRepository;
    }

    public async Task<ExamDto> Handle(GetExamByIdQuery request, CancellationToken cancellationToken)
    {
        var exam = await _examRepository.GetByIdAsync<Exam>(request.ExamId);
        return exam is null ? new ExamDto() : new ExamDto
        {
            Id = exam.Id,
            ClassId = exam.ClassId,
            SubjectName = exam.SubjectName,
            ExamName = exam.ExamName,
            ExamType = exam.ExamType,
            ExamDate = exam.ExamDate,
            StartTime = exam.StartTime,
            EndTime = exam.EndTime,
            Duration = exam.Duration,
            TotalMarks = exam.TotalMarks,
            PassingMarks = exam.PassingMarks,
            IsPublished = exam.IsPublished,
            CreatedAt = exam.CreatedAt
        };
    }
}

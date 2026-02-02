using Application.Common.Interfaces.Repositories;
using Application.Features.SchoolClassManagement.ExamManagement.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Features.SchoolClassManagement.ExamManagement.Queries.GetAllExams;

public class GetExamByIdQueryHandler : IRequestHandler<GetAllExamsQuery, List<ExamDto>>
{
    private readonly IExamRepository _examRepository;
    public GetExamByIdQueryHandler(IExamRepository examRepository)
    {
        _examRepository = examRepository;
    }

    public async Task<List<ExamDto>> Handle(GetAllExamsQuery request, CancellationToken cancellationToken)
    {
        var exams = await _examRepository.GetAllAsync<Exam>();
        var examDtos = exams.Select(exam => new ExamDto
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
        }).ToList();
        return examDtos;
    }
}

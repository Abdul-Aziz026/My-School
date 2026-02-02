
using Application.Common.Interfaces.Repositories;
using Application.Features.SchoolClassManagement.StudentManagement.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Features.SchoolClassManagement.StudentManagement.Queries.GetExamResults;

public class GetExamResultsQueryHandler : IRequestHandler<GetExamResultsQuery, List<ExamResultDto>>
{
    private readonly IExamRepository _examRepository;
    private readonly IStudentRepository _studentRepository;
    public GetExamResultsQueryHandler(IExamRepository examRepository, IStudentRepository studentRepository)
    {
        _examRepository = examRepository;
        _studentRepository = studentRepository;
    }

    public async Task<List<ExamResultDto>> Handle(GetExamResultsQuery request, CancellationToken cancellationToken)
    {
        var results = await _examRepository.GetItemsByConditionAsync<ExamResult>(e => e.ExamId == request.ExamId);
        return results?.Select(o => new ExamResultDto()
        {
            ExamId = o.ExamId,
            StudentId = o.StudentId,
            TotalMarks = o.TotalMarks,
            ObtainedMarks= o.ObtainedMarks,
            IsPassed = o.IsPassed,
            Percentage = o.Percentage,
            EvaluatedAt = o.EvaluatedAt
        }).ToList()!;
    }
}

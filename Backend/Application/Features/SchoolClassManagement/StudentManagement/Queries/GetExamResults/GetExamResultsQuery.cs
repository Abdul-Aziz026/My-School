
using Application.Features.SchoolClassManagement.StudentManagement.DTOs;
using MediatR;

namespace Application.Features.SchoolClassManagement.StudentManagement.Queries.GetExamResults;

public class GetExamResultsQuery : IRequest<List<ExamResultDto>>
{
    public string ExamId { get; set; }
}

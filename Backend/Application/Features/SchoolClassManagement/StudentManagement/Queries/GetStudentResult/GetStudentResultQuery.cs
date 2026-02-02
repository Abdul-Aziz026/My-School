
using Application.Features.SchoolClassManagement.StudentManagement.DTOs;
using MediatR;

namespace Application.Features.SchoolClassManagement.StudentManagement.Queries.GetStudentResult;

public class GetStudentResultQuery : IRequest<ExamResultDto>
{
    public string StudentId { get; set; }
    public string ExamId { get; set; }
}

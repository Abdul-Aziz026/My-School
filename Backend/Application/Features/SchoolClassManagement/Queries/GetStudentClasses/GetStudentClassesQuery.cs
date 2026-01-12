
using Application.Features.SchoolClassManagement.DTOs;
using MediatR;

namespace Application.Features.SchoolClassManagement.Queries.GetStudentClasses;

public class GetStudentClassesQuery : IRequest<List<ClassResponseDto>>
{
    public string StudentId { get; set; } = string.Empty;
    public GetStudentClassesQuery(string studentId)
    {
        StudentId = studentId;
    }
}

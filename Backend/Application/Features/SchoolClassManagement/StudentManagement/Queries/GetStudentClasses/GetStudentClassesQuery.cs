using Application.Features.SchoolClassManagement.ClassManagement.DTOs;
using MediatR;

namespace Application.Features.SchoolClassManagement.StudentManagement.Queries.GetStudentClasses;

public class GetStudentClassesQuery : IRequest<List<ClassResponseDto>>
{
    public string StudentId { get; set; } = string.Empty;
    public GetStudentClassesQuery(string studentId)
    {
        StudentId = studentId;
    }
}

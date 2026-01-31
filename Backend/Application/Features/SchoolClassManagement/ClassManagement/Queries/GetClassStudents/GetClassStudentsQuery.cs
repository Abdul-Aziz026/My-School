using Application.Features.SchoolClassManagement.StudentManagement.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Features.SchoolClassManagement.ClassManagement.Queries.GetClassStudents;

public class GetClassStudentsQuery : IRequest<List<StudentResponseDto>>
{
    public string ClassId { get; set; }
    public GetClassStudentsQuery(string classId)
    {
        ClassId = classId;
    }
}

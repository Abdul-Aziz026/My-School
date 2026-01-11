
using Application.Features.SchoolClassManagement.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Features.SchoolClassManagement.Queries.GetClassStudents;

public class GetClassStudentsQuery : IRequest<List<StudentResponseDto>>
{
    public string ClassId { get; set; }
    public GetClassStudentsQuery(string classId)
    {
        ClassId = classId;
    }
}

using Application.Features.SchoolClassManagement.TeacherManagement.DTOs;
using MediatR;

namespace Application.Features.SchoolClassManagement.Queries.GetClassTeachers;

public class GetClassTeachersQuery : IRequest<List<TeacherResponseDto>>
{
    public string Id { get; set; }

    public GetClassTeachersQuery(string id)
    {
        Id = id;
    }
}
using Application.Features.SchoolClassManagement.TeacherManagement.DTOs;
using MediatR;

namespace Application.Features.SchoolClassManagement.TeacherManagement.Queries.GetTeacherById;

public class GetTeacherByIdQuery : IRequest<TeacherResponseDto>
{
    public string Id { get; set; }

    public GetTeacherByIdQuery(string id)
    {
        Id = id;
    }
}
using Application.Features.SchoolClassManagement.DTOs;
using MediatR;

namespace Application.Features.SchoolClassManagement.Queries.GetTeacherById;

public class GetTeacherByIdQuery : IRequest<TeacherResponseDto>
{
    public string Id { get; set; }

    public GetTeacherByIdQuery(string id)
    {
        Id = id;
    }
}
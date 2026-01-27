using Application.Features.SchoolClassManagement.DTOs;
using MediatR;

namespace Application.Features.SchoolClassManagement.Queries.GetStudentById;

public class GetStudentByIdQuery : IRequest<StudentResponseDto>
{
    public string Id { get; set; }

    public GetStudentByIdQuery(string id)
    {
        Id = id;
    }
}
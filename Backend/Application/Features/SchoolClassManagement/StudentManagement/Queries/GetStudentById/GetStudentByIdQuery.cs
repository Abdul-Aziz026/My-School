using Application.Features.SchoolClassManagement.StudentManagement.DTOs;
using MediatR;

namespace Application.Features.SchoolClassManagement.StudentManagement.Queries.GetStudentById;

public class GetStudentByIdQuery : IRequest<StudentResponseDto>
{
    public string Id { get; set; }

    public GetStudentByIdQuery(string id)
    {
        Id = id;
    }
}
using Application.Features.SchoolClassManagement.ClassManagement.DTOs;
using MediatR;

namespace Application.Features.SchoolClassManagement.ClassManagement.Queries.GetClassById;

public class GetClassByIdQuery : IRequest<ClassResponseDto>
{
    public string Id { get; set; }
    public GetClassByIdQuery(string id)
    {
        Id = id;
    }
}

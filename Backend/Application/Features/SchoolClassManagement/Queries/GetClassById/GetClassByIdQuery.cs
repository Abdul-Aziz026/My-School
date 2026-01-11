
using Application.Features.SchoolClassManagement.DTOs;
using MediatR;

namespace Application.Features.SchoolClassManagement.Queries.GetClassById;

public class GetClassByIdQuery : IRequest<ClassResponseDto>
{
    public string Id { get; set; }
    public GetClassByIdQuery(string id)
    {
        Id = id;
    }
}

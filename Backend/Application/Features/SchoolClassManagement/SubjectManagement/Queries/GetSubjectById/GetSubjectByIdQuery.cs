using Application.Features.SchoolClassManagement.SubjectManagement.DTOs;
using MediatR;

namespace Application.Features.SchoolClassManagement.Queries.GetSubjectById;

public class GetSubjectByIdQuery : IRequest<SubjectResponseDto>
{
    public string Id { get; set; }

    public GetSubjectByIdQuery(string id)
    {
        Id = id;
    }
}
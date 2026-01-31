using Application.Features.SchoolClassManagement.ClassManagement.DTOs;
using MediatR;

namespace Application.Features.SchoolClassManagement.Queries.GetSubjectClasses;

public class GetSubjectClassesQuery : IRequest<List<ClassResponseDto>>
{
    public string SubjectId { get; set; }
}
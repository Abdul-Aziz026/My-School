
using Application.Features.SchoolClassManagement.DTOs;
using MediatR;

namespace Application.Features.SchoolClassManagement.Queries.GetTeacherClasses;

public class GetTeacherClassesQuery : IRequest<List<ClassResponseDto>>
{
    public string TeacherId { get; set; }
    public GetTeacherClassesQuery(string teacherId)
    {
        TeacherId = teacherId;
    }
}

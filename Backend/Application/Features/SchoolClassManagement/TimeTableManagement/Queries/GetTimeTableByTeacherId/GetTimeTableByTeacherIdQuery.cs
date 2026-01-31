using Application.Features.SchoolClassManagement.TimeTableManagement.DTOs;
using MediatR;

namespace Application.Features.SchoolClassManagement.TimeTableManagement.Queries.GetTimeTableByTeacherId;

public class GetTimeTableByTeacherIdQuery : IRequest<IEnumerable<TimeTableResponseDto>>
{
    public string TeacherId { get; set; }
    public GetTimeTableByTeacherIdQuery(string teacherId)
    {
        TeacherId = teacherId;
    }
}

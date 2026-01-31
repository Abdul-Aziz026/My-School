
using Application.Features.SchoolClassManagement.TimeTableManagement.DTOs;
using MediatR;

namespace Application.Features.SchoolClassManagement.TimeTableManagement.Queries.GetTimeTableByClassAndDay;

public class GetTimeTableByClassAndDayQuery : IRequest<IEnumerable<TimeTableResponseDto>>
{
    public string ClassId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public GetTimeTableByClassAndDayQuery(string classId, DayOfWeek dayOfWeek)
    {
        ClassId = classId;
        DayOfWeek = dayOfWeek;
    }
}

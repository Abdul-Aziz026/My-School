using Application.Features.SchoolClassManagement.TimeTableManagement.DTOs;
using MediatR;

namespace Application.Features.SchoolClassManagement.TimeTableManagement.Queries.GetTimeTableByClassId;

public class GetTimeTableByClassIdQuery : IRequest<IEnumerable<TimeTableResponseDto>>
{
    public string ClassId { get; set; }
    public GetTimeTableByClassIdQuery(string classId)
    {
        ClassId = classId;
    }
}

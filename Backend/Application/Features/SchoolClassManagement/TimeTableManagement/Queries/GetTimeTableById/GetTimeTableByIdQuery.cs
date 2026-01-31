
using Application.Features.SchoolClassManagement.TimeTableManagement.DTOs;
using MediatR;

namespace Application.Features.SchoolClassManagement.TimeTableManagement.Queries.GetTimeTableById;

public class GetTimeTableByIdQuery : IRequest<TimeTableResponseDto>
{
    public string TimeTableId { get; set; }
    public GetTimeTableByIdQuery(string id)
    {
        TimeTableId = id;
    }
}

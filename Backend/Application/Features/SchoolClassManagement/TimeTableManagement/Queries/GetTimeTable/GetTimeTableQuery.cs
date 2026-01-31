using Application.Features.SchoolClassManagement.TimeTableManagement.DTOs;
using MediatR;

namespace Application.Features.SchoolClassManagement.TimeTableManagement.Queries.GetTimeTable;

public class GetTimeTableQuery : IRequest<IEnumerable<TimeTableResponseDto>>
{
}

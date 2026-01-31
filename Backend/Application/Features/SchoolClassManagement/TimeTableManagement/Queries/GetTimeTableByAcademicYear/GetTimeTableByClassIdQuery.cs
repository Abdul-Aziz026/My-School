using Application.Features.SchoolClassManagement.TimeTableManagement.DTOs;
using MediatR;

namespace Application.Features.SchoolClassManagement.TimeTableManagement.Queries.GetTimeTableByAcademicYear;

public class GetTimeTableByAcademicYearQuery : IRequest<IEnumerable<TimeTableResponseDto>>
{
    public string AcademicYear { get; set; }
    public GetTimeTableByAcademicYearQuery(string academicYear)
    {
        AcademicYear = academicYear;
    }
}

using Application.Features.SchoolClassManagement.AttendanceManagement.DTOs;
using MediatR;

namespace Application.Features.SchoolClassManagement.AttendanceManagement.Queries.ExportAttendanceData;

public class ExportAttendanceDataQuery : IRequest<AttendanceFileResultDto>
{
    public string Format { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? ClassId { get; set; }
}
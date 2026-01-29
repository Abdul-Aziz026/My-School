using Application.Features.SchoolClassManagement.AttendanceManagement.DTOs;
using MediatR;

namespace Application.Features.SchoolClassManagement.AttendanceManagement.Queries.GetStudentAttendanceHistory;

public class GetStudentAttendanceHistoryQuery : IRequest<List<AttendanceResponseDto>>
{
    public string StudentId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
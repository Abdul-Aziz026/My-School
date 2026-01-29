using Application.Features.SchoolClassManagement.AttendanceManagement.DTOs;
using MediatR;

namespace Application.Features.SchoolClassManagement.AttendanceManagement.Queries.GetDailyAttendance;

public class 
    
    GetDailyAttendanceQuery : IRequest<List<AttendanceResponseDto>>
{
    public string ClassId { get; set; }
    public DateTime Date { get; set; }
}
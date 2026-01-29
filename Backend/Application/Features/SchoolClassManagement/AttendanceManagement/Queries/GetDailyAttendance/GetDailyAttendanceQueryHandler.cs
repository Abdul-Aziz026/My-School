using System.Linq.Expressions;
using Application.Common.Interfaces.Repositories;
using Application.Features.SchoolClassManagement.AttendanceManagement.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Features.SchoolClassManagement.AttendanceManagement.Queries.GetDailyAttendance;

public class GetDailyAttendanceQueryHandler : IRequestHandler<GetDailyAttendanceQuery, List<AttendanceResponseDto>>
{
    private readonly IAttendanceRepository _attendanceRepository;
    public GetDailyAttendanceQueryHandler(IAttendanceRepository attendanceRepository)
    {
        _attendanceRepository = attendanceRepository;
    }

    public async Task<List<AttendanceResponseDto>> Handle(GetDailyAttendanceQuery query, CancellationToken cancellationToken)
    {
        Expression<Func<Attendance, bool>> filter = x => x.ClassId == query.ClassId && x.Date.Date == query.Date.Date;
        var attendanceResponses = await _attendanceRepository.GetItemsByConditionAsync(filter);
        var attendanceResponseDtos = attendanceResponses?.Select(o =>
            new AttendanceResponseDto()
            {
                AttendanceId = o.Id,
                ClassId = o.ClassId,
                ClassName = "o.ClassName",
                StudentId = o.StudentId,
                StudentName = "o.StudentName",
                Date = o.Date,
                EntryTime = o.EntryTime,
                ExitTime = o.ExitTime!.Value,
                IsPresent = o.IsPresent
            }
        ).ToList();
        return attendanceResponseDtos!;
    }
}
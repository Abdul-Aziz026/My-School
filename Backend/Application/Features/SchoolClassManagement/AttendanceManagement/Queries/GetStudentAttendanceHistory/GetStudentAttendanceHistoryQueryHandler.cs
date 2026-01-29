using System.Linq.Expressions;
using Application.Common.Extensions;
using Application.Common.Interfaces.Repositories;
using Application.Features.SchoolClassManagement.AttendanceManagement.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Features.SchoolClassManagement.AttendanceManagement.Queries.GetStudentAttendanceHistory;

public class GetStudentAttendanceHistoryQueryHandler(IAttendanceRepository attendanceRepository) 
                                                     : IRequestHandler<GetStudentAttendanceHistoryQuery, List<AttendanceResponseDto>>
{
    public async Task<List<AttendanceResponseDto>> Handle(GetStudentAttendanceHistoryQuery query, CancellationToken cancellationToken)
    {
        Expression<Func<Attendance, bool>> filter = x => x.StudentId == query.StudentId;
        if (query.StartDate.HasValue)
        {
            filter = filter.And(x => x.Date.Date >= query.StartDate.Value.Date);
        }
        if (query.EndDate.HasValue) {
            filter = filter.And(x => x.Date.Date <= query.EndDate.Value.Date);
        }

        var studentAttendanceResponses = await attendanceRepository.GetItemsByConditionAsync<Attendance>(filter);
        var attendanceResponseDtos = studentAttendanceResponses?.Select(o =>
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

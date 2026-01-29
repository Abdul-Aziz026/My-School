
using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using MediatR;
using System.Linq.Expressions;

namespace Application.Features.SchoolClassManagement.AttendanceManagement.Commands.RecordExit;

public class RecordExitCommandHandler : IRequestHandler<RecordExitCommand>
{
    private readonly IAttendanceRepository _attendanceRepository;

    public RecordExitCommandHandler(IAttendanceRepository attendanceRepository)
    {
        _attendanceRepository = attendanceRepository;
    }

    public async Task Handle(RecordExitCommand request, CancellationToken cancellationToken)
    {
        // Find today entry record
        Expression<Func<Attendance, bool>> filter = x => true;
        var attendance = await _attendanceRepository.GetItemByConditionAsync<Attendance>(filter);
        if (attendance == null)
        {
            throw new NotFoundException("No entry record found for this student today. Please record entry first.");
        }
        if (attendance?.ExitTime != null)
        {
            throw new InvalidOperationException("Exit time already recorded for this student today");
        }

        attendance!.ExitTime = request.ExitTime;
        var updated = await _attendanceRepository.UpdateAsync(attendance);
        if (!updated)
        {
            throw new Exception("Failed to update the exit time");
        }
    }
}

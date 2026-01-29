using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using MediatR;
using MongoDB.Driver;

namespace Application.Features.SchoolClassManagement.AttendanceManagement.Commands.UpdateAttendance;

public class UpdateAttendanceCommandHandler : IRequestHandler<UpdateAttendanceCommand>
{
    private readonly IAttendanceRepository _attendanceRepository;

    public UpdateAttendanceCommandHandler(IAttendanceRepository attendanceRepository)
    {
        _attendanceRepository = attendanceRepository;
    }

    public async Task Handle(UpdateAttendanceCommand command, CancellationToken cancellationToken)
    {
        var attendance = await _attendanceRepository.GetByIdAsync<Attendance>(command.AttendanceId);
        if (attendance == null) {
            throw new NotFoundException($"Attendance record with ID {command.AttendanceId} not found");
        }

        if (command.EntryTime.HasValue)
        {
            attendance.EntryTime = command.EntryTime.Value;
        }
        if (command.ExitTime.HasValue){
            if (attendance.EntryTime > command.ExitTime.Value)
            {
                throw new InvalidOperationException("Exit time cannot be earlier than entry time");
            }
            attendance.ExitTime = command.ExitTime.Value;
        }

        if (command.Date.HasValue)
        {
            attendance.Date = command.Date.Value;
        }

        var updated = await _attendanceRepository.UpdateAsync(attendance);
        if (!updated)
        {
            throw new Exception("Attendance update failed");
        }
    }
}
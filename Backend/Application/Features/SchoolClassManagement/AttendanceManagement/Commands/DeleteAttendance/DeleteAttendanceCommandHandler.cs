using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Features.SchoolClassManagement.AttendanceManagement.Commands.DeleteAttendance;

public class DeleteAttendanceCommandHandler : IRequestHandler<DeleteAttendanceCommand>
{
    private readonly IAttendanceRepository _attendanceRepository;

    public DeleteAttendanceCommandHandler(IAttendanceRepository attendanceRepository)
    {
        _attendanceRepository = attendanceRepository;
    }

    public async Task Handle(DeleteAttendanceCommand command, CancellationToken cancellationToken)
    {
        var attendance = await _attendanceRepository.GetByIdAsync<Attendance>(command.AttendanceId);
        if (attendance is null)
        {
            throw new NotFoundException("Attendance not found");
        }

        var isDeleted = await _attendanceRepository.DeleteByIdAsync<Attendance>(command.AttendanceId);
        if (!isDeleted)
        {
            throw new Exception("Attendance deleted failed!");
        }
    }
}
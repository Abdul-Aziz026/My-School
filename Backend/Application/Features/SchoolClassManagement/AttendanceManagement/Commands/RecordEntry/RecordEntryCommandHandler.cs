
using MediatR;
using Domain.Entities;
using Application.Features.SchoolClassManagement.AttendanceManagement.Commands.RecordEntry;
using Application.Common.Interfaces.Repositories;

namespace Application.Features.SchoolClassManagement.AttendanceManagement.Commands.RecordEntry;

public class RecordEntryCommandHandler : IRequestHandler<RecordEntryCommand>
{
    private readonly IAttendanceRepository _attendanceRepository;
    public RecordEntryCommandHandler(IAttendanceRepository attendanceRepository)
    {
        _attendanceRepository = attendanceRepository;
    }

    public async Task Handle(RecordEntryCommand command, CancellationToken cancellationToken)
    {
        var attendance = new Attendance()
        {
            StudentId = command.StudentId,
            EntryTime = command.EntryTime,
            IsPresent = true,
        };
        await _attendanceRepository.AddAsync<Attendance>(attendance);
    }
}

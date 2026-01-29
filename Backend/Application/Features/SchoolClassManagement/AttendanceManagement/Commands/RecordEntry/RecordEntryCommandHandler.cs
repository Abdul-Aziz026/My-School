
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using Application.Common.Exceptions;
using MediatR;
using Domain.Entities;
using Application.Features.SchoolClassManagement.AttendanceManagement.Commands.RecordEntry;
using Application.Common.Interfaces.Repositories;
using Application.Common.Extensions;

namespace Application.Features.SchoolClassManagement.AttendanceManagement.Commands.RecordEntry;

public class RecordEntryCommandHandler : IRequestHandler<RecordEntryCommand>
{
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IClassRepository _classRepository;
    public RecordEntryCommandHandler(IAttendanceRepository attendanceRepository,
                                     IStudentRepository studentRepository,
                                     IClassRepository classRepository)
    {
        _attendanceRepository = attendanceRepository;
        _studentRepository = studentRepository;
        _classRepository = classRepository;
    }

    public async Task Handle(RecordEntryCommand command, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByIdAsync<Student>(command.StudentId);
        if (student == null)
        {
            throw new NotFoundException("Student not found!");
        }

        var schoolClass = await _studentRepository.GetByIdAsync<Class>(command.ClassId);
        if (schoolClass == null)
        {
            throw new NotFoundException($"Class with ID {command.ClassId} not found");
        }

        Expression<Func<Attendance, bool>> filter = x => x.StudentId == command.StudentId
                                                         && x.ClassId == command.ClassId
                                                         && x.Date == DateTime.UtcNow.Date;
        var existingAttendance = await _attendanceRepository.GetItemByConditionAsync<Attendance>(filter);
        if (existingAttendance != null)
        {
            throw new InvalidOperationException(
                "Attendance entry already exists for this student today");
        }

        var attendance = new Attendance()
        {
            StudentId = command.StudentId,
            EntryTime = command.EntryTime,
            Date = DateTime.UtcNow.Date,
            IsPresent = true,
        };
        await _attendanceRepository.AddAsync<Attendance>(attendance);
    }
}

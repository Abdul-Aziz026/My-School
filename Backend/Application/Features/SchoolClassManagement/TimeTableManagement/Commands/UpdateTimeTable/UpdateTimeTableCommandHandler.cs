using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Application.Features.SchoolClassManagement.TimeTableManagement.DTOs;
using Domain.Entities;
using MediatR;
using System.Linq.Expressions;

namespace Application.Features.SchoolClassManagement.TimeTableManagement.Commands.UpdateTimeTable;

public class UpdateTimeTableCommandHandler : IRequestHandler<UpdateTimeTableCommand, TimeTableResponseDto>
{
    private readonly ITimeTableRepository _timeTableRepository;
    private readonly ISubjectRepository _subjectRepository;
    private readonly ITeacherRepository _teacherRepository;
    private readonly IClassRepository _classRepository;
    public UpdateTimeTableCommandHandler(ITimeTableRepository timeTableRepository,
                                    ISubjectRepository subjectRepository,
                                    ITeacherRepository teacherRepository, IClassRepository classRepository)
    {
        _timeTableRepository = timeTableRepository;
        _subjectRepository = subjectRepository;
        _teacherRepository = teacherRepository;
        _classRepository = classRepository;
    }
    public async Task<TimeTableResponseDto> Handle(UpdateTimeTableCommand request, CancellationToken cancellationToken)
    {
        var existing = await _timeTableRepository.GetByIdAsync<TimeTable>(request.Id);
        if (existing == null)
        {
            throw new NotFoundException($"Timetable with ID {request.Id} not found.");
        }

        // Check for conflicts (excluding current entry)
        Expression<Func<TimeTable, bool>> filter = tt =>
            tt.Id != request.Id &&
            tt.ClassId == request.ClassId &&
            tt.DayOfWeek == request.DayOfWeek &&
            tt.TimeSlotId == request.TimeSlotId && 
            tt.AcademicYear == request.AcademicYear;
        var conflict = await _timeTableRepository.GetItemByConditionAsync<TimeTable>(filter);
        if (conflict is not null)
        {
            throw new ArgumentException("A timetable entry already exists for this class, day, and time slot.");
        }

        // check teacher availability
        filter = tt =>
            tt.Id != request.Id &&
            tt.TeacherId == request.TeacherId &&
            tt.DayOfWeek == request.DayOfWeek &&
            tt.TimeSlotId == request.TimeSlotId &&
            tt.AcademicYear == request.AcademicYear;
        var teacherConflict = await _timeTableRepository.GetItemByConditionAsync<TimeTable>(filter);
        if (teacherConflict is not null) {
            throw new ArgumentException("The selected teacher is not available at the specified time slot.");
        }

        // check room availability
        filter = tt =>
            tt.Id != request.Id &&
            tt.RoomNumber.ToString() == request.RoomNumber &&
            tt.DayOfWeek == request.DayOfWeek &&
            tt.TimeSlotId == request.TimeSlotId &&
            tt.AcademicYear == request.AcademicYear;
        var roomConflict = await _timeTableRepository.GetItemByConditionAsync<TimeTable>(filter);
        if (roomConflict is not null) {
            throw new ArgumentException("The selected room is already booked at the specified time slot.");
        }

        // Fetch related entities to populate names
        var subject = await _subjectRepository.GetByIdAsync<Subject>(request.SubjectId);
        var teacher = await _teacherRepository.GetByIdAsync<Teacher>(request.TeacherId);
        var schoolClass = await _classRepository.GetByIdAsync<Class>(request.ClassId);

        if (subject == null || teacher == null || schoolClass == null)
        {
            throw new ArgumentException("Invalid SubjectId, TeacherId, or ClassId provided.");
        }

        // Update properties
        existing.TimeSlotId = request.TimeSlotId;
        existing.SubjectId = request.SubjectId;
        existing.SubjectName = subject?.Name ?? string.Empty;
        existing.TeacherId = request.TeacherId;
        existing.TeacherName = teacher?.Name ?? string.Empty;
        existing.ClassId = request.ClassId;
        existing.ClassName = schoolClass?.Name ?? string.Empty;
        existing.DayOfWeek = request.DayOfWeek;
        existing.RoomNumber = Enum.TryParse<RoomNumber>(request.RoomNumber, true, out var room) ? room : default;
        existing.AcademicYear = request.AcademicYear;

        var updated = await _timeTableRepository.UpdateAsync<TimeTable>(existing);
        return new TimeTableResponseDto
        {
            Id = existing.Id,
            TimeSlotId = existing.TimeSlotId,
            SubjectId = existing.SubjectId,
            SubjectName = existing.SubjectName,
            TeacherId = existing.TeacherId,
            TeacherName = existing.TeacherName,
            ClassId = existing.ClassId,
            ClassName = existing.ClassName,
            DayOfWeek = existing.DayOfWeek,
            RoomNumber = existing.RoomNumber.ToString(),
            AcademicYear = existing.AcademicYear
        };
    }

}

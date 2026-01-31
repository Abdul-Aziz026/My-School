
using Application.Common.Interfaces.Repositories;
using Application.Features.SchoolClassManagement.TimeTableManagement.DTOs;
using Domain.Entities;
using MediatR;
using System.Linq.Expressions;
using System.Xml;

namespace Application.Features.SchoolClassManagement.TimeTableManagement.Commands.CreateTimeTable;

public class CreateTimeTableCommandHandler : IRequestHandler<CreateTimeTableCommand, string>
{
    private readonly ITimeTableRepository _timeTableRepository;
    private readonly ISubjectRepository _subjectRepository;
    private readonly ITeacherRepository _teacherRepository;
    private readonly IClassRepository _classRepository;
    public CreateTimeTableCommandHandler(ITimeTableRepository timeTableRepository, 
                                    ISubjectRepository subjectRepository, 
                                    ITeacherRepository teacherRepository, IClassRepository classRepository)
    {
        _timeTableRepository = timeTableRepository;
        _subjectRepository = subjectRepository;
        _teacherRepository = teacherRepository;
        _classRepository = classRepository;
    }

    public async Task<string> Handle(CreateTimeTableCommand request, CancellationToken cancellationToken)
    {
        // Check for conflicts (same class, day, and timeslot)
        Expression<Func<TimeTable, bool>> filter = x => 
                                    x.ClassId == request.ClassId &&
                                    x.DayOfWeek == request.DayOfWeek &&
                                    x.TimeSlotId == request.TimeSlotId &&
                                    x.AcademicYear == request.AcademicYear;
        var conflictTimeTable = await _timeTableRepository.GetItemByConditionAsync<TimeTable>(filter);
        if (conflictTimeTable is not null)
        {
            throw new ArgumentException("A timetable entry already exists for the specified class, day, and timeslot.");
        }
        // Check if teacher is available
        filter = x => x.TeacherId == request.TeacherId &&
                    x.DayOfWeek == request.DayOfWeek &&
                    x.TimeSlotId == request.TimeSlotId &&
                    x.AcademicYear == request.AcademicYear;
        var teacherConflict = await _timeTableRepository.GetItemByConditionAsync<TimeTable>(filter);
        if (teacherConflict is not null)
        {
            throw new ArgumentException("The specified teacher is not available for the given timeslot.");
        }
        // Check room availability
        filter = x => x.RoomNumber.ToString() == request.RoomNo &&
                    x.DayOfWeek == request.DayOfWeek &&
                    x.TimeSlotId == request.TimeSlotId &&
                    x.AcademicYear == request.AcademicYear;
        var roomConflict = await _timeTableRepository.GetItemByConditionAsync<TimeTable>(filter);
        if (roomConflict is not null)
        {
            throw new ArgumentException("The specified room is already booked for the given timeslot.");
        }
        // Fetch related entities to populate names
        var subject = await _subjectRepository.GetByIdAsync<Subject>(request.SubjectId);
        var teacher = await _teacherRepository.GetByIdAsync<Teacher>(request.TeacherId);
        var schoolClass = await _classRepository.GetByIdAsync<Class>(request.ClassId);

        if (subject == null || teacher == null || schoolClass == null)
        {
            throw new ArgumentException("Invalid SubjectId, TeacherId, or ClassId provided.");
        }

        var timeTable = new TimeTable
        {
            TimeSlotId = request.TimeSlotId,
            SubjectId = request.SubjectId,
            SubjectName = subject.Name,
            TeacherId = request.TeacherId,
            TeacherName = teacher.Name,
            ClassId = request.ClassId,
            ClassName = schoolClass.Name,
            DayOfWeek = request.DayOfWeek,
            RoomNumber = Enum.TryParse<RoomNumber>(request.RoomNo, true, out var room) ? room : default ,
            AcademicYear = request.AcademicYear
        };

        var created = await _timeTableRepository.AddAsync<TimeTable>(timeTable);
        return timeTable.Id;
    }
}

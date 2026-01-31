
using Application.Common.Interfaces.Repositories;
using Application.Features.SchoolClassManagement.TimeTableManagement.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Features.SchoolClassManagement.TimeTableManagement.Queries.GetTimeTable;

public class GetTimeTableQueryHandler : IRequestHandler<GetTimeTableQuery, IEnumerable<TimeTableResponseDto>>
{
    private readonly ITimeTableRepository _timeTableRepository;
    public GetTimeTableQueryHandler(ITimeTableRepository timeTableRepository)
    {
        _timeTableRepository = timeTableRepository;
    }

    public async Task<IEnumerable<TimeTableResponseDto>> Handle(GetTimeTableQuery request, CancellationToken cancellationToken)
    {
        var timeTables = await _timeTableRepository.GetAllAsync<TimeTable>();
        List<TimeTableResponseDto> timeTableResponseDtos = new();
        foreach (var timeTable in timeTables)
        {
            var timeSlot = await _timeTableRepository.GetByIdAsync<TimeSlot>(timeTable.TimeSlotId);
            timeTableResponseDtos.Add(ToTimeTableResponseDto(timeTable, timeSlot));
        }
        return timeTableResponseDtos;
    }

    private static TimeTableResponseDto ToTimeTableResponseDto(TimeTable timeTable, TimeSlot? timeSlot)
    {
        return new TimeTableResponseDto
        {
            Id = timeTable.Id,
            TimeSlotId = timeSlot.Id,
            SlotName = timeSlot.SlotName,
            StartTime = timeSlot.StartTime.ToString("HH:mm"),
            EndTime = timeSlot.EndTime.ToString("HH:mm"),
            Duration = (timeSlot.EndTime - timeSlot.StartTime).TotalMinutes.ToString(),
            SubjectId = timeTable.SubjectId,
            SubjectName = timeTable.SubjectName,
            TeacherId = timeTable.TeacherId,
            TeacherName = timeTable.TeacherName,
            ClassId = timeTable.ClassId,
            ClassName = timeTable.ClassName,
            DayOfWeek = timeTable.DayOfWeek,
            RoomNumber = nameof(timeTable.RoomNumber),
            AcademicYear = timeTable.AcademicYear
        };
    }
}

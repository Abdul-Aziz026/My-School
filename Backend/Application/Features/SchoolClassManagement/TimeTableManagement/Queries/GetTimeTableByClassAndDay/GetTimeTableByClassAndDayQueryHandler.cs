using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Application.Features.SchoolClassManagement.TimeTableManagement.DTOs;
using Domain.Entities;
using MediatR;
using System.Linq.Expressions;

namespace Application.Features.SchoolClassManagement.TimeTableManagement.Queries.GetTimeTableByClassAndDay;

public class GetTimeTableByClassAndDayQueryHandler : IRequestHandler<GetTimeTableByClassAndDayQuery, IEnumerable<TimeTableResponseDto>>
{
    private readonly ITimeTableRepository _timeTableRepository;
    public GetTimeTableByClassAndDayQueryHandler(ITimeTableRepository timeTableRepository)
    {
        _timeTableRepository = timeTableRepository;
    }
    public async Task<IEnumerable<TimeTableResponseDto>> Handle(GetTimeTableByClassAndDayQuery query, CancellationToken cancellationToken)
    {
        Expression<Func<TimeTable, bool>> filter = tt => tt.ClassId == query.ClassId && tt.DayOfWeek == query.DayOfWeek;
        var timeTables = await _timeTableRepository.GetItemsByConditionAsync<TimeTable>(filter);
        List<TimeTableResponseDto> timeTableResponseDtos = new();
        foreach (var timeTable in timeTables)
        {
            var timeSlot = await _timeTableRepository.GetByIdAsync<TimeSlot>(timeTable.TimeSlotId);
            if (timeSlot is null)
            {
                throw new Exception($"TimeSlot with Id {timeTable.TimeSlotId} not found.");
            }
            timeTableResponseDtos.Add(ToTimeTableResponseDto(timeTable, timeSlot));
        }
        return timeTableResponseDtos;
    }

    private static TimeTableResponseDto ToTimeTableResponseDto(TimeTable timeTable, TimeSlot timeSlot)
    {
        return new TimeTableResponseDto
        {
            Id = timeTable.Id,
            TimeSlotId = timeSlot.Id,
            SlotName = timeSlot.SlotName,
            StartTime = timeSlot.StartTime.ToString("hh\\:mm"),
            EndTime = timeSlot.EndTime.ToString("hh\\:mm"),
            Duration = (timeSlot.EndTime - timeSlot.StartTime).TotalMinutes.ToString(),
            SubjectId = timeTable.SubjectId,
            SubjectName = timeTable.SubjectName,
            TeacherId = timeTable.TeacherId,
            TeacherName = timeTable.TeacherName,
            ClassId = timeTable.ClassId,
            ClassName = timeTable.ClassName,
            DayOfWeek = timeTable.DayOfWeek,
            RoomNumber = timeTable.RoomNumber.ToString(),
            AcademicYear = timeTable.AcademicYear
        };
    }
}

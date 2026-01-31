using Application.Common.Interfaces.Repositories;
using Application.Features.SchoolClassManagement.TimeTableManagement.DTOs;
using Domain.Entities;
using MediatR;
using System.Linq.Expressions;

namespace Application.Features.SchoolClassManagement.TimeTableManagement.Queries.GetTimeTableByAcademicYear;

public class GetTimeTableByAcademicYearQueryHandler : IRequestHandler<GetTimeTableByAcademicYearQuery, IEnumerable<TimeTableResponseDto>>
{
    private readonly ITimeTableRepository _timeTableRepository;
    public GetTimeTableByAcademicYearQueryHandler(ITimeTableRepository timeTableRepository)
    {
        _timeTableRepository = timeTableRepository;
    }
    public async Task<IEnumerable<TimeTableResponseDto>> Handle(GetTimeTableByAcademicYearQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<TimeTable, bool>> filter = tt => tt.AcademicYear == request.AcademicYear;
        var timeTables = await _timeTableRepository.GetItemsByConditionAsync(filter);
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

    private TimeTableResponseDto ToTimeTableResponseDto(TimeTable timeTable, TimeSlot timeSlot)
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
            RoomNumber = nameof(timeTable.RoomNumber),
            AcademicYear = timeTable.AcademicYear
        };
    }
}

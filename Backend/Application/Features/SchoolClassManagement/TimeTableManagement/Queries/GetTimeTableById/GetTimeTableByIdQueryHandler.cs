using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Application.Features.SchoolClassManagement.TimeTableManagement.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Features.SchoolClassManagement.TimeTableManagement.Queries.GetTimeTableById;

public class GetTimeTableByIdQueryHandler : IRequestHandler<GetTimeTableByIdQuery, TimeTableResponseDto>
{
    private readonly ITimeTableRepository _timeTableRepository;
    public GetTimeTableByIdQueryHandler(ITimeTableRepository timeTableRepository)
    {
        _timeTableRepository = timeTableRepository;
    }
    public async Task<TimeTableResponseDto> Handle(GetTimeTableByIdQuery query, CancellationToken cancellationToken)
    {
        var timeTable = await _timeTableRepository.GetByIdAsync<TimeTable>(query.TimeTableId);
        if (timeTable == null)
        {
            throw new NotFoundException("time table not found");
        }
        var timeSlot = await _timeTableRepository.GetByIdAsync<TimeSlot>(timeTable.TimeSlotId);
        if (timeSlot == null)
        {
            throw new NotFoundException("time slot not found");
        }
        TimeTableResponseDto timeTableDto = ToTimeTableResponseDto(timeTable, timeSlot);
        return timeTableDto;
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

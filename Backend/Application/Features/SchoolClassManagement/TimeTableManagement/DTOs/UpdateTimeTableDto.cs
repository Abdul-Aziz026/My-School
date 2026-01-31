using Application.Features.SchoolClassManagement.TimeTableManagement.Commands.UpdateTimeTable;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.SchoolClassManagement.TimeTableManagement.DTOs;

public class UpdateTimeTableDto
{
    public string TimeSlotId { get; set; }
    public string SubjectId { get; set; }
    public string TeacherId { get; set; }
    public string ClassId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public string RoomNumber { get; set; }
    public string AcademicYear { get; set; }

    public UpdateTimeTableCommand ToUpdateTimeTableCommand(string id)
    {
        return new UpdateTimeTableCommand
        {
            Id = id,
            TimeSlotId = this.TimeSlotId,
            SubjectId = this.SubjectId,
            TeacherId = this.TeacherId,
            ClassId = this.ClassId,
            DayOfWeek = this.DayOfWeek,
            RoomNumber = this.RoomNumber,
            AcademicYear = this.AcademicYear
        };
    }
}

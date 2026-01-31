using Application.Features.SchoolClassManagement.TimeTableManagement.Commands.CreateTimeTable;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.Features.SchoolClassManagement.TimeTableManagement.DTOs;

public class CreateTimeTableDto
{
    [Required(ErrorMessage = "TimeSlotId can't be empty")]
    public string TimeSlotId { get; set; } = string.Empty;
    [Required(ErrorMessage = "SubjectId can't be empty")]
    public string SubjectId { get; set; } = string.Empty;
    [Required(ErrorMessage = "SubjectId can't be empty")]
    public string ClassId { get; set; } = string.Empty;
    [Required(ErrorMessage = "ClassId can't be empty")]
    public string TeacherId { get; set; } = string.Empty;
    [Required(ErrorMessage = "Day can't be empty")]
    public DayOfWeek DayOfWeek { get; set; }
    [Required(ErrorMessage = "Room Number can't be empty")]
    public string RoomNo { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;

    public CreateTimeTableCommand ToCreateTimeTableCommand()
    {
        return new CreateTimeTableCommand
        {
            TimeSlotId = this.TimeSlotId,
            TeacherId = this.TeacherId,
            SubjectId = this.SubjectId,
            ClassId = this.ClassId,
            DayOfWeek = this.DayOfWeek,
            RoomNo = this.RoomNo,
            AcademicYear = this.AcademicYear            
        };
    }
}

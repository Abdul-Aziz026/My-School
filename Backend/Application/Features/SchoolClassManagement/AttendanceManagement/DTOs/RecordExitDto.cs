using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Application.Features.SchoolClassManagement.AttendanceManagement.Commands.RecordExit;

namespace Application.Features.SchoolClassManagement.AttendanceManagement.DTOs;

public class RecordExitDto
{

    [Required(ErrorMessage = "StudentId is required.")]
    public string StudentId { get; set; } = string.Empty;
    [Required(ErrorMessage = "EntryTime is required.")]
    public DateTime ExitTime { get; set; }
    [Required(ErrorMessage = "ClassId is required.")]
    public string ClassId { get; set; } = string.Empty;

    public RecordExitCommand ToRecordExitCommand()
    {
        return new RecordExitCommand()
        {
            StudentId = this.StudentId,
            ExitTime = this.ExitTime,
            ClassId = this.ClassId
        };
    }
}

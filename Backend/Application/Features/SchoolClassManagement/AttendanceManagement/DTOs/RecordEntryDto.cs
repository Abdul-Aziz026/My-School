
using Application.Features.SchoolClassManagement.AttendanceManagement.Commands.RecordEntry;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.SchoolClassManagement.AttendanceManagement.DTOs;

public class RecordEntryDto
{
    [Required(ErrorMessage = "StudentId is required.")]
    public string StudentId { get; set; } = string.Empty;
    [Required(ErrorMessage = "EntryTime is required.")]
    public DateTime EntryTime { get; set; }
    [Required(ErrorMessage = "ClassId is required.")]
    public string ClassId { get; set; } = string.Empty;

    public RecordEntryCommand ToRecordEntryCommand()
    {
        return new RecordEntryCommand
        {
            StudentId = this.StudentId,
            EntryTime = this.EntryTime,
            ClassId = this.ClassId
        };
    }
}

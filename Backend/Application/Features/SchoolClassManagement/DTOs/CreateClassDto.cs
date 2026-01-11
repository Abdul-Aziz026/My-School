using Application.Features.SchoolClassManagement.Commands.CreateClass;
using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.SchoolClassManagement.DTOs;

public class CreateClassDto
{
    [Required]
    public string SchoolId { get; set; } = string.Empty;
    [Required]
    public string Name { get; set; } = string.Empty;
    public int Grade { get; set; }
    public string Section { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public List<string> Subjects { get; set; } = new();
    public List<string> TeacherIds { get; set; } = new();

    public CreateClassCommand ToCreateClassCommand()     {
        return new CreateClassCommand()
        {
            SchoolId = this.SchoolId,
            Name = this.Name,
            Grade = this.Grade,
            Section = this.Section,
            AcademicYear = this.AcademicYear,
            Capacity = this.Capacity,
            Subjects = this.Subjects,
            TeacherIds = this.TeacherIds
        };
    }
}

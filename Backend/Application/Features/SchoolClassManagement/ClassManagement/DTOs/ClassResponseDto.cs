using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.SchoolClassManagement.ClassManagement.DTOs;

public class ClassResponseDto
{
    public string Id { get; set; }
    public string SchoolId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Grade { get; set; }
    public string Section { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public List<string> Subjects { get; set; } = new();
    public List<string> TeacherIds { get; set; } = new();
}

public static class ClassResposeExtension
{
    public static ClassResponseDto ToClassResponseDto(this Class obj)
    {
        return new ClassResponseDto
        {
            Id = obj.Id,
            SchoolId = obj.SchoolId,
            Name = obj.Name,
            Grade = obj.Grade,
            Section = obj.Section,
            AcademicYear = obj.AcademicYear,
            Capacity = obj.Capacity,
            Subjects = obj.Subjects,
            TeacherIds = obj.TeacherIds
        };
    }
}

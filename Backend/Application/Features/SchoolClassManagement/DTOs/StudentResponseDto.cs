
using Domain.Entities;

namespace Application.Features.SchoolClassManagement.DTOs;

public class StudentResponseDto
{
    public string Name { get; set; } = string.Empty;
    public int Grade { get; set; }
    public string Section { get; set; } = string.Empty;
}

public static class StudentResponseDtoExtentions
{
    public static StudentResponseDto ToStudentResponseDto(this Student student)
    {
        return new StudentResponseDto
        {
            Name = student.Name,
            Grade = student.Grade,
            Section = student.Section
        };
    }
}

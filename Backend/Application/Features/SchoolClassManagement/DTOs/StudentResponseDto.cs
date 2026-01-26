
using Domain.Entities;

namespace Application.Features.SchoolClassManagement.DTOs;

public sealed class StudentResponseDto
{
    public string Id { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string StudentNumber { get; init; } = default!;
    public int Grade { get; init; }
    public string Section { get; init; } = default!;
    public StudentStatus Status { get; init; }
}

public static class StudentResponseDtoExtentions
{
    public static StudentResponseDto ToStudentResponseDto(this Student student)
    {
        return new StudentResponseDto
        {
            Id = student.Id,
            StudentNumber = student.StudentNumber,
            Name = student.Name,
            Grade = student.Grade,
            Section = student.Section,
            Status = student.Status
        };
    }
}

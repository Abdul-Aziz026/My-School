using Domain.Entities;

namespace Application.Features.SchoolClassManagement.TeacherManagement.DTOs;

public class TeacherResponseDto
{
    public string Id { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string EmployeeNumber { get; init; } = default!;
    public string Department { get; init; } = default!;
    public TeacherStatus Status { get; init; }
}

public static class ToTeacherResponseDtoExtensions
{
    public static TeacherResponseDto ToTeacherResponseDto(this Teacher teacher)
    {
        return new TeacherResponseDto()
        {
            Id = teacher.Id,
            Name = teacher.Name,
            EmployeeNumber = teacher.EmployeeNumber,
            Department = teacher.Department,
            Status = teacher.Status,
        };
    }
}
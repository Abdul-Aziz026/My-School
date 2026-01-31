using Application.Features.SchoolClassManagement.StudentManagement.Commands.UpdateStudent;
using Domain.Entities;

namespace Application.Features.SchoolClassManagement.StudentManagement.DTOs;

public class UpdateStudentDto
{
    public string? ClassId { get; set; }
    public string? Name { get; set; }
    public string? StudentNumber { get; set; }
    public string? SchoolId { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public int? Grade { get; set; }
    public string? Section { get; set; }
    public StudentStatus? Status { get; set; }

    public UpdateStudentCommand ToUpdateStudentCommand(string id)
    {
        return new UpdateStudentCommand()
        {
            Id = id,
            ClassId = this.ClassId,
            Name = this.Name,
            StudentNumber = this.StudentNumber,
            SchoolId = this.SchoolId,
            DateOfBirth = this.DateOfBirth,
            Email = this.Email,
            Phone = this.Phone,
            Grade = this.Grade,
            Section = this.Section,
            Status = this.Status
        };
    }
}
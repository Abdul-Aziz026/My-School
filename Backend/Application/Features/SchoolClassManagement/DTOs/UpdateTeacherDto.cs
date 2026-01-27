
using Application.Features.SchoolClassManagement.Commands.UpdateTeacher;
using Domain.Entities;

namespace Application.Features.SchoolClassManagement.DTOs;

public class UpdateTeacherDto
{
    public string Name { get; set; } = string.Empty;
    public string SchoolId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string EmployeeNumber { get; set; } = string.Empty;
    public TeacherStatus Status { get; set; }
    public DateTime HireDate { get; set; }
    public string Department { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty; // e.g. Senior Teacher

    public UpdateTeacherCommand ToUpdateTeacherCommand(string id)
    {
        return new UpdateTeacherCommand() { 
            Id = id,
            Name = this.Name,
            SchoolId = this.SchoolId,
            Email = this.Email,
            Phone = this.Phone,
            EmployeeNumber = this.EmployeeNumber,
            Status = this.Status,
            HireDate = this.HireDate,
            Department = this.Department,
            Designation = this.Designation,
        };
    }
}

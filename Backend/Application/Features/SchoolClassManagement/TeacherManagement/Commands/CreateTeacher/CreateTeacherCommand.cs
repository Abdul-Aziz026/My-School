using Domain.Entities;
using MediatR;

namespace Application.Features.SchoolClassManagement.TeacherManagement.Commands.CreateTeacher;

public class CreateTeacherCommand : IRequest<string>
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
}

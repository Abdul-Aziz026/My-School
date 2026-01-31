using Domain.Entities;
using MediatR;

namespace Application.Features.SchoolClassManagement.TeacherManagement.Commands.UpdateTeacher;

public class UpdateTeacherCommand : IRequest
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string SchoolId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string EmployeeNumber { get; set; } = string.Empty;
    public TeacherStatus Status { get; set; }
    public DateTime HireDate { get; set; }

    // Academic
    public string Department { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty; // e.g. Senior Teacher
}

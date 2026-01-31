using MediatR;
using System;

namespace Application.Features.SchoolClassManagement.ClassManagement.Commands.UpdateClass;

public class UpdateClassCommand : IRequest
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
    public bool IsActive { get; set; }
}

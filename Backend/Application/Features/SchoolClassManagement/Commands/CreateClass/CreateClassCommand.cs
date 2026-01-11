using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.SchoolClassManagement.Commands.CreateClass;

public class CreateClassCommand : IRequest<string>
{
    public string SchoolId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Grade { get; set; }
    public string Section { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public List<string> Subjects { get; set; } = new();
    public List<string> TeacherIds { get; set; } = new();
}


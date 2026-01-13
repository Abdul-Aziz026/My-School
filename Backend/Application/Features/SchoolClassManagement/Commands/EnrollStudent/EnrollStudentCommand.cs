
using MediatR;

namespace Application.Features.SchoolClassManagement.Commands.EnrollStudent;

/// <summary>
/// return Enrollment Id
/// </summary>
/// <param name="StudentId"></param>
/// <param name="ClassId"></param>
public class EnrollStudentCommand : IRequest<string>
{
    public string StudentId { get; set; } = string.Empty;
    public string ClassId { get; set; } = string.Empty;
    public List<string> SubjectIds { get; set; } = new();
}

using MediatR;

namespace Application.Features.SchoolClassManagement.Commands.EnrollStudent;

/// <summary>
/// return Enrollment Id
/// </summary>
/// <param name="StudentId"></param>
/// <param name="ClassId"></param>
public record EnrollStudentCommand(string StudentId, string ClassId) : IRequest<string>;


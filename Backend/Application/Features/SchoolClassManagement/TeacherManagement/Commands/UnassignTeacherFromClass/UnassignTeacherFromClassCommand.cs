using MediatR;

namespace Application.Features.SchoolClassManagement.TeacherManagement.Commands.UnassignTeacherFromClass;

public class UnassignTeacherFromClassCommand : IRequest
{
    public string TeacherId { get; set; } = string.Empty;
    public string ClassId { get; set; } = string.Empty;
}

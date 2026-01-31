using MediatR;

namespace Application.Features.SchoolClassManagement.TeacherManagement.Commands.AssignTeacherToClass;

public class AssignTeacherToClassCommand : IRequest<string>
{
    public string TeacherId { get; set; }
    public string ClassId { get; set; }
}

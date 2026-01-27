
using Application.Features.SchoolClassManagement.Commands.AssignTeacherToClass;

namespace Application.Features.SchoolClassManagement.DTOs;

public class AssignTeacherToClassDto
{
    public string TeacherId { get; set; }
    public string ClassId { get; set; }

    public AssignTeacherToClassCommand ToAssignTeacherToClassCommand()
    {
        return new AssignTeacherToClassCommand() {
            TeacherId = TeacherId,
            ClassId = ClassId
        };
    }
}

using Application.Features.SchoolClassManagement.TeacherManagement.Commands.AssignTeacherToClass;

namespace Application.Features.SchoolClassManagement.TeacherManagement.DTOs;

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

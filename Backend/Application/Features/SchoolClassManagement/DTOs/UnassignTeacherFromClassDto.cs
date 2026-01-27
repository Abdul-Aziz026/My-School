
using Application.Features.SchoolClassManagement.Commands.UnassignTeacherFromClass;

namespace Application.Features.SchoolClassManagement.DTOs;

public class UnassignTeacherFromClassDto
{
    public string TeacherId { get; set; } = string.Empty;
    public string ClassId { get; set; } = string.Empty;

    public UnassignTeacherFromClassCommand ToUnassignTeacherFromClassCommand()
    {
        return new UnassignTeacherFromClassCommand
        {
            TeacherId = this.TeacherId,
            ClassId = this.ClassId
        };
    }
}

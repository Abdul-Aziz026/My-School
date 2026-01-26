using MediatR;

namespace Application.Features.SchoolClassManagement.Commands.AssignTeacher;

public class AssignTeacherCommand : IRequest
{
    public string ClassId { get; set; }
    public string TeacherId { get; set; }

    public AssignTeacherCommand(string classId, string teacherId)
    {
        ClassId = classId;
        TeacherId = teacherId;
    }
}
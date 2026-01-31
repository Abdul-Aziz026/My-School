using MediatR;

namespace Application.Features.SchoolClassManagement.Commands.RemoveTeacher;

public class RemoveTeacherCommand : IRequest
{
    public string StudentId { get; set; }
    public string TeacherId { get; set; }

    public RemoveTeacherCommand(string studentId, string teacherId)
    {
        StudentId = studentId;
        TeacherId = teacherId;
    }
}
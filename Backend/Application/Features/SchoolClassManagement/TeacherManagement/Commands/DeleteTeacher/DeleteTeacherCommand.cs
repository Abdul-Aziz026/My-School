using MediatR;

namespace Application.Features.SchoolClassManagement.TeacherManagement.Commands.DeleteTeacher;

public class DeleteTeacherCommand : IRequest
{
    public string TeacherId { get; set; }
    public DeleteTeacherCommand(string id)
    {
        TeacherId = id;
    }
}

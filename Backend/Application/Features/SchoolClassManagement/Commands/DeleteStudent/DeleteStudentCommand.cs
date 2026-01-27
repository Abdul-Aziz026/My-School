using MediatR;

namespace Application.Features.SchoolClassManagement.Commands.DeleteStudent;

public class DeleteStudentCommand : IRequest
{
    public string Id { get; set; }

    public DeleteStudentCommand(string id)
    {
        Id = id;
    }
}
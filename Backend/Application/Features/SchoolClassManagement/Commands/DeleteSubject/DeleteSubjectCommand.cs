using MediatR;

namespace Application.Features.SchoolClassManagement.Commands.DeleteSubject;

public class DeleteSubjectCommand : IRequest
{
    public string Id { get; set; }

    public DeleteSubjectCommand(string id)
    {
        Id = id;
    }
}
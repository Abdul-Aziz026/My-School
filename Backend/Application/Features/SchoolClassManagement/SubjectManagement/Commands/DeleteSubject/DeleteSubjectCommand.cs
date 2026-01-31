using MediatR;

namespace Application.Features.SchoolClassManagement.SubjectManagement.Commands.DeleteSubject;

public class DeleteSubjectCommand : IRequest
{
    public string Id { get; set; }

    public DeleteSubjectCommand(string id)
    {
        Id = id;
    }
}
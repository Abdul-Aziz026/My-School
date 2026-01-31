using MediatR;

namespace Application.Features.SchoolClassManagement.ClassManagement.Commands.DeleteClass;

public class DeleteClassCommand : IRequest
{
    public string Id { get; set; }
    public DeleteClassCommand(string id)
    {
        Id = id;
    }
}


using MediatR;

namespace Application.Features.SchoolClassManagement.Commands.DeleteClass;

public class DeleteClassCommand : IRequest
{
    public string Id { get; set; }
    public DeleteClassCommand(string id)
    {
        Id = id;
    }
}

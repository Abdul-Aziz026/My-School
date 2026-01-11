
using MediatR;

namespace Application.Features.SchoolClassManagement.Commands.DeleteClass;

public class DeleteClassCommandHandler : IRequestHandler<DeleteClassCommand>
{
    public Task Handle(DeleteClassCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

using MediatR;
using System;

namespace Application.Features.SchoolClassManagement.Commands.UpdateClass;

public class UpdateClassCommandHandler : IRequestHandler<UpdateClassCommand>
{
    public Task Handle(UpdateClassCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

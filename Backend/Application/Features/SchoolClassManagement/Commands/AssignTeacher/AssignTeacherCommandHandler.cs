using MediatR;

namespace Application.Features.SchoolClassManagement.Commands.AssignTeacher;

public class AssignTeacherCommandHandler : IRequestHandler<AssignTeacherCommand>
{
    public async Task Handle(AssignTeacherCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
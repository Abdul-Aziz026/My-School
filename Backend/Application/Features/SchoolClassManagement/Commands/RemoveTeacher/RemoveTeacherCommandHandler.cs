using Application.Common.Interfaces.Repositories;
using MediatR;

namespace Application.Features.SchoolClassManagement.Commands.RemoveTeacher;

public class RemoveTeacherCommandHandler : IRequestHandler<RemoveTeacherCommand>
{
    private readonly IClassRepository _classRepository;

    public RemoveTeacherCommandHandler(IClassRepository classRepository)
    {
        _classRepository = classRepository;
    }

    public async Task Handle(RemoveTeacherCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Features.SchoolClassManagement.Commands.DeleteClass;

public class DeleteClassCommandHandler : IRequestHandler<DeleteClassCommand>
{
    private readonly IClassRepository _classRepository;
    public DeleteClassCommandHandler(IClassRepository classRepository)
    {
        _classRepository = classRepository;
    }
    public async Task Handle(DeleteClassCommand request, CancellationToken cancellationToken)
    {
        bool isDeleted = await _classRepository.DeleteByIdAsync<User>(request.Id);
        if (!isDeleted)
        {
            throw new ArgumentException("User not found");
        }
        return;
    }
}

using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Features.SchoolClassManagement.ClassManagement.Commands.DeleteClass;

public class DeleteClassCommandHandler : IRequestHandler<DeleteClassCommand>
{
    private readonly IClassRepository _classRepository;
    public DeleteClassCommandHandler(IClassRepository classRepository)
    {
        _classRepository = classRepository;
    }
    public async Task Handle(DeleteClassCommand request, CancellationToken cancellationToken)
    {
        var id = request.Id;
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentNullException($"Id can't be null or empty!!!");
        }
        var deleteClass = await _classRepository.GetByIdAsync<Class>(id);
        if (deleteClass == null)
        {
            throw new NotFoundException("class not found");
        }
        deleteClass.IsActive = false;
        bool isDeleted = await _classRepository.UpdateAsync<Class>(deleteClass);
        if (!isDeleted)
        {
            throw new Exception("Uuknown exception");
        }
        return;
    }
}

using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Features.SchoolClassManagement.Commands.DeleteSubject;

public class DeleteSubjectCommandHandler : IRequestHandler<DeleteSubjectCommand>
{
    private readonly ISubjectRepository _subjectRepository;

    public DeleteSubjectCommandHandler(ISubjectRepository subjectRepository)
    {
        _subjectRepository = subjectRepository;
    }

    public async Task Handle(DeleteSubjectCommand request, CancellationToken cancellationToken)
    {
        var subject = await _subjectRepository.GetByIdAsync<Subject>(request.Id);
        if (subject == null)
        {
            throw new NotFoundException("Subject not found");
        }

        subject.IsActive = false;
        var isUpdate = await _subjectRepository.UpdateAsync(subject);
        if (!isUpdate)
        {
            throw new Exception("Failed to delete subject");
        }
    }
}
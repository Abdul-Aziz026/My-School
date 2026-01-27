using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Features.SchoolClassManagement.Commands.UpdateSubject;

public class UpdateSubjectCommandHandler : IRequestHandler<UpdateSubjectCommand>
{
    private readonly ISubjectRepository _subjectRepository;

    public UpdateSubjectCommandHandler(ISubjectRepository subjectRepository)
    {
        _subjectRepository = subjectRepository;
    }

    public async Task Handle(UpdateSubjectCommand request, CancellationToken cancellationToken)
    {
        var subject = await _subjectRepository.GetByIdAsync<Subject>(request.Id);
        if (subject is null)
        {
            throw new NotFoundException("Subject not found");
        }
        ApplyUpdateSubjectProperty(request, subject);
        await _subjectRepository.UpdateAsync(subject);
    }

    private static void ApplyUpdateSubjectProperty(UpdateSubjectCommand request, Subject subject)
    {
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            subject.Name = request.Name;
        }
        if (!string.IsNullOrWhiteSpace(request.Code))
        {
            subject.Code = request.Code;
        }if (!string.IsNullOrWhiteSpace(request.Description))
        {
            subject.Description = request.Description;
        }
        if (request.Credits.HasValue)
        {
            subject.Credits = request.Credits.Value;
        }
        subject.IsActive = request.IsActive;
    }
}
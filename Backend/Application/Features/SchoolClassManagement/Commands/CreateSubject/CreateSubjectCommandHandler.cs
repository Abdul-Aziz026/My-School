
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using Domain.Repositories.Base;
using MediatR;
using System.Linq.Expressions;

namespace Application.Features.SchoolClassManagement.Commands.CreateSubject;

public class CreateSubjectCommandHandler : IRequestHandler<CreateSubjectCommand, string>
{
    private readonly ISubjectRepository _subjectRepository;
    public CreateSubjectCommandHandler(ISubjectRepository subjectRepository)
    {
        _subjectRepository = subjectRepository;
    }
    public async Task<string> Handle(CreateSubjectCommand command, CancellationToken cancellationToken)
    {
        Expression<Func<Subject, bool>> condition = x => x.Name == command.Name;
        var subject = await _subjectRepository.GetItemByConditionAsync<Subject>(condition);
        if (subject is not null)
        {
            throw new InvalidOperationException("Subject Already Exists");
        }
        var newSubject = new Subject
        {
            Name = command.Name,
            Code = command.Code,
            Credits = command.Credits,
            Description = command.Description,
        };
        await _subjectRepository.DeleteAsync<Subject>(newSubject);
        return newSubject.Id;
    }
}

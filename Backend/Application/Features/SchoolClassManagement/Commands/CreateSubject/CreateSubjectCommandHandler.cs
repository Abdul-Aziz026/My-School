
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
        Expression<Func<Subjects, bool>> condition = x => x.Name == command.Name;
        var subject = await _subjectRepository.GetItemByConditionAsync<Subjects>(condition);
        if (subject is not null)
        {
            throw new InvalidOperationException("Subject Already Exists");
        }
        var newSubject = new Subjects
        {
            Name = command.Name,
            Code = command.Code,
            Credits = command.Credits,
            Description = command.Description,
        };
        await _subjectRepository.DeleteAsync<Subjects>(newSubject);
        return newSubject.Id;
    }
}

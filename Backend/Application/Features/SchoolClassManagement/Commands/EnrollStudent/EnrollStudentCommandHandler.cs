
using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using Domain.Entities.JunctionEntities;
using MediatR;
using System.Linq.Expressions;

namespace Application.Features.SchoolClassManagement.Commands.EnrollStudent;

public class EnrollStudentCommandHandler : IRequestHandler<EnrollStudentCommand, string>
{
    private readonly IClassRepository _classRepository;
    public EnrollStudentCommandHandler(IClassRepository classRepository)
    {
        _classRepository = classRepository;
    }
    public async Task<string> Handle(EnrollStudentCommand command, CancellationToken cancellationToken)
    {
        var classEntity = await _classRepository.GetByIdAsync<Class>(command.ClassId);
        if (classEntity is null) {
            throw new NotFoundException("Class not found");
        }
        var studentEntity = await _classRepository.GetByIdAsync<Student>(command.StudentId);
        if (studentEntity is null)
        {
            throw new NotFoundException("Student not found");
        }
        foreach (var Id in command.SubjectIds)
        {
            var subject = await _classRepository.GetByIdAsync<Subjects>(Id);
            if (subject is null)
            {
                throw new NotFoundException("Subject not found");
            }
        }
        Expression<Func<ClassStudentEnrollment, bool>> condition = x => x.StudentId == command.StudentId 
                                                                        && x.ClassId == command.ClassId;
        var enrollmentResponse = await _classRepository.GetItemByConditionAsync<ClassStudentEnrollment>(condition);
        if (enrollmentResponse is null)
        {
            throw new InvalidOperationException("Student already enrolled!");
        }
        var enrollment = new ClassStudentEnrollment
        {
            Id = Guid.NewGuid().ToString(),
            StudentId = command.StudentId,
            ClassId = command.ClassId,
            Status = EnrollMentStatus.Active,
            SubjectIds = command.SubjectIds,
            EnrolledAt = DateTime.UtcNow
        };
        var response = await _classRepository.AddAsync<ClassStudentEnrollment>(enrollment);
        if (!response)
        {
            throw new Exception("unknown Exception");
        }
        return enrollment.Id;
    }
}

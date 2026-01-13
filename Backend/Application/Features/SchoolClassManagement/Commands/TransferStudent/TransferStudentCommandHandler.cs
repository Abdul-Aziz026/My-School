
using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using Domain.Entities.JunctionEntities;
using MediatR;
using System.Linq.Expressions;

namespace Application.Features.SchoolClassManagement.Commands.TransferStudent;

public class TransferStudentCommandHandler : IRequestHandler<TransferStudentCommand, string>
{
    private readonly IClassRepository _classRepository;
    public TransferStudentCommandHandler(IClassRepository classRepository)
    {
        _classRepository = classRepository;
    }
    public async Task<string> Handle(TransferStudentCommand command, CancellationToken cancellationToken)
    {
        var studentId = command.StudentId;
        var toClassId = command.ToClassId;
        var fromClassId = command.FromClassId;

        if (studentId is null)
        {
            throw new ArgumentNullException(nameof(studentId));
        }
        if (toClassId is null)
        {
            throw new ArgumentNullException(nameof(toClassId));
        }
        if (fromClassId is null)
        {
            throw new ArgumentNullException(nameof(fromClassId));
        }
        Expression<Func<ClassStudentEnrollment, bool>> condition = x => x.StudentId == studentId &&
                                                                        x.ClassId == toClassId;
        var oldEnrollment = await _classRepository.GetItemByConditionAsync<ClassStudentEnrollment>(condition);
        if (oldEnrollment is null) {
            throw new NotFoundException($"Student {studentId} is not enrolled in class {fromClassId}");
        }

        var student = await _classRepository.GetByIdAsync<Student>(studentId);
        if (student == null)
        {
            throw new NotFoundException("student not found");
        }

        var fromClass = await _classRepository.GetByIdAsync<Class>(fromClassId);
        if (fromClass == null)
        {
            throw new NotFoundException("From class not found");
        }

        var newClass = await _classRepository.GetByIdAsync<Class>(toClassId);
        if (newClass == null)
        {
            throw new NotFoundException("New class not found");
        }
        if (newClass.IsActive is false)
        {
            throw new InvalidOperationException("New class is inactive");
        }
        if (newClass.StudentIds.Count == newClass.Capacity) {
            throw new InvalidOperationException("New class is full");
        }

        foreach (var Id in command.SubjectIds)
        {
            var subject = await _classRepository.GetByIdAsync<Subjects>(Id);
            if (subject is null)
            {
                throw new NotFoundException("Subject not found");
            }
        }

        var newEnrollment = new ClassStudentEnrollment
        {
            ClassId = newClass.Id,
            StudentId = studentId,
            EnrolledAt = DateTime.UtcNow,
            Status = EnrollMentStatus.Active,
            SubjectIds = command.SubjectIds
        };

        await _classRepository.AddAsync<ClassStudentEnrollment>(newEnrollment);
        return newEnrollment.Id;
    }
}

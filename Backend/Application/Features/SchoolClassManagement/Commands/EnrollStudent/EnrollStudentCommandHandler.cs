
using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
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
    public async Task<string> Handle(EnrollStudentCommand request, CancellationToken cancellationToken)
    {
        var classEntity = await _classRepository.GetByIdAsync<Class>(request.ClassId);
        if (classEntity is null) {
            throw new NotFoundException("Class not found");
        }
        var studentEntity = await _classRepository.GetByIdAsync<Student>(request.StudentId);
        if (studentEntity is null)
        {
            throw new NotFoundException("Student not found");
        }
        Expression<Func<ClassStudentEnrollment, bool>> condition = x => x.StudentId == request.StudentId 
                                                                        && x.ClassId == request.ClassId;
        var enrollmentResponse = await _classRepository.GetItemByConditionAsync<ClassStudentEnrollment>(condition);
        if (enrollmentResponse is null)
        {
            throw new InvalidOperationException("Student already enrolled!");
        }
        var enrollment = new ClassStudentEnrollment
        {
            Id = Guid.NewGuid().ToString(),
            StudentId = request.StudentId,
            ClassId = request.ClassId,
            Status = EnrollMentStatus.Active,
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

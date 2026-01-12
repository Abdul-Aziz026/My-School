
using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using MediatR;
using System.Linq.Expressions;

namespace Application.Features.SchoolClassManagement.Commands.UnenrollStudent;

public class UnenrollStudentCommandHandler : IRequestHandler<UnenrollStudentCommand>
{
    private readonly IClassRepository _classRepository;
    public UnenrollStudentCommandHandler(IClassRepository classRepository)
    {
        _classRepository = classRepository;
    }
    public async Task Handle(UnenrollStudentCommand request, CancellationToken cancellationToken)
    {
        var classEntity = await _classRepository.GetByIdAsync<Class>(request.ClassId);
        if (classEntity is null)
        {
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
            throw new InvalidOperationException("Student already not enrolled!");
        }
        var response = await _classRepository.DeleteByIdAsync<ClassStudentEnrollment>(enrollmentResponse.Id);
        if (!response)
        {
            throw new Exception("unknown Exception");
        }
    }
}

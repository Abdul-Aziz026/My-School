using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using Domain.Entities.JunctionEntities;
using MediatR;

namespace Application.Features.SchoolClassManagement.TeacherManagement.Commands.AssignTeacherToClass;

public class AssignTeacherToClassCommandHandler : IRequestHandler<AssignTeacherToClassCommand, string>
{
    private readonly ITeacherRepository _teacherRepository;
    private readonly IClassRepository _classRepository;
    public AssignTeacherToClassCommandHandler(ITeacherRepository teacherRepository, IClassRepository classRepository)
    {
        _teacherRepository = teacherRepository;
        _classRepository = classRepository;
    }

    public async Task<string> Handle(AssignTeacherToClassCommand request, CancellationToken cancellationToken)
    {
        var teacher = await _teacherRepository.GetByIdAsync<Teacher>(request.TeacherId);
        var classEntity = await _classRepository.GetByIdAsync<Class>(request.ClassId);
        if (teacher is null)
        {
            throw new NotFoundException("Teacher not found");
        }
        if (classEntity is null)
        {
            throw new NotFoundException("Class not found");
        }
        var assignment = new TeacherClassAssignment()
        {
            TeacherId = request.TeacherId,
            ClassId = request.ClassId
        };
        var response = await _teacherRepository.AddAsync(assignment);
        if (response  is false)
        {
            throw new Exception("Failed to assign teacher to class");
        }
        return assignment.Id;
    }
}

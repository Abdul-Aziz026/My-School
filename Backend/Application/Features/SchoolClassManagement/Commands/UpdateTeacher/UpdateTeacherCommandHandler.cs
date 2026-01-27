
using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Features.SchoolClassManagement.Commands.UpdateTeacher;

public class UpdateTeacherCommandHandler : IRequestHandler<UpdateTeacherCommand>
{
    private readonly ITeacherRepository _teacherRepository;
    public UpdateTeacherCommandHandler(ITeacherRepository teacherRepository)
    {
        _teacherRepository = teacherRepository;
    }
    public async Task Handle(UpdateTeacherCommand request, CancellationToken cancellationToken)
    {
        var teacher = await _teacherRepository.GetByIdAsync<Teacher>(request.Id);
        if (teacher == null)
        {
            throw new NotFoundException("Teacher not found");
        }
        ApplyTeacherUpdateProperty(request, teacher);
        await _teacherRepository.UpdateAsync(teacher);
    }

    private static void ApplyTeacherUpdateProperty(UpdateTeacherCommand request, Teacher teacher)
    {
        if (!string.IsNullOrEmpty(request.Name))
        {
            teacher.Name = request.Name;
        }
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            teacher.Email = request.Email;
        }
        if (!string.IsNullOrEmpty(request.EmployeeNumber))
        {
            teacher.EmployeeNumber = request.EmployeeNumber;
        }
        if (!string.IsNullOrWhiteSpace(request.Phone))
        {
            teacher.Phone = request.Phone;
        }
        if (!string.IsNullOrWhiteSpace(request.Department))
        {
            teacher.Department = request.Department;
        }

        if (!string.IsNullOrWhiteSpace(request.Designation))
        {
            teacher.Designation = request.Designation;
        }
    }
}

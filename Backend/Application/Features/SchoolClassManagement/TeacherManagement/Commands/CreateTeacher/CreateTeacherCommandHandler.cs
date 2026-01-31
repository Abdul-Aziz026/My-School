using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Features.SchoolClassManagement.TeacherManagement.Commands.CreateTeacher;

public class CreateTeacherCommandHandler : IRequestHandler<CreateTeacherCommand, string>
{
    private readonly ITeacherRepository _teacherRepository;
    public CreateTeacherCommandHandler(ITeacherRepository teacherRepository)
    {
        _teacherRepository = teacherRepository;
    }
    public async Task<string> Handle(CreateTeacherCommand request, CancellationToken cancellationToken)
    {
        var teacher = new Teacher
        {
            Name = request.Name,
            SchoolId = request.SchoolId,
            Email = request.Email,
            Phone = request.Phone,
            EmployeeNumber = request.EmployeeNumber,
            Status = request.Status,
            HireDate = request.HireDate,
            Department = request.Department,
            Designation = request.Designation
        };
        var createdTeacher = await _teacherRepository.AddAsync(teacher);
        if (createdTeacher is false)
        {
            throw new Exception("Failed to create teacher.");
        }
        return teacher.Id;
    }
}

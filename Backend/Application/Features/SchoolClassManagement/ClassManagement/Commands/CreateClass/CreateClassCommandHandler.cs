using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using MediatR;
using System;

namespace Application.Features.SchoolClassManagement.ClassManagement.Commands.CreateClass;

public class CreateClassCommandHandler : IRequestHandler<CreateClassCommand, string>
{
    private readonly IClassRepository _classRepository;
    public CreateClassCommandHandler(IClassRepository classRepository)
    {
        _classRepository = classRepository;
    }

    public async Task<string> Handle(CreateClassCommand request, CancellationToken cancellationToken)
    {
        var newClass = new Class
        {
            SchoolId = request.SchoolId,
            Name = request.Name,
            Grade = request.Grade,
            Section = request.Section,
            AcademicYear = request.AcademicYear,
            Capacity = request.Capacity,
            Subjects = request.Subjects,
            TeacherIds = request.TeacherIds,
            StudentIds = new List<string>(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var createdClass = await _classRepository.AddAsync<Class>(newClass);

        return newClass.Id;
    }
}

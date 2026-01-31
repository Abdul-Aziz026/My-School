using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using MediatR;
using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.SchoolClassManagement.ClassManagement.Commands.UpdateClass;

public class UpdateClassCommandHandler : IRequestHandler<UpdateClassCommand>
{
    private readonly IClassRepository _classRepository;
    public UpdateClassCommandHandler(IClassRepository classRepository)
    {
        _classRepository = classRepository;
    }

    public async Task Handle(UpdateClassCommand request, CancellationToken cancellationToken)
    {
        var existingClass = await _classRepository.GetByIdAsync<Class>(request.Id);

        if (existingClass == null)
        {
            throw new KeyNotFoundException($"Class with ID {request.Id} not found");
        }

        ApplyUpdateClassAsync(request, existingClass);
        var response = await _classRepository.UpdateAsync<Class>(existingClass);
        if (response != true)
        {
            throw new Exception("Update failed");
        }
        return;
    }

    private void ApplyUpdateClassAsync(UpdateClassCommand request, Class existingClass)
    {
        if (!string.IsNullOrWhiteSpace(request.SchoolId))
        {
            existingClass.SchoolId = request.SchoolId;
        }
        if (!string.IsNullOrWhiteSpace(request.Name)) { 
            existingClass.Name = request.Name;
        }
        if (request.Capacity > 0)
        {
            existingClass.Capacity = request.Capacity;
        }
        if (!string.IsNullOrWhiteSpace(request.Section))
        {
            existingClass.Section = request.Section;
        }
        if (request.Grade > 0)
        {
            existingClass.Grade = request.Grade;
        }
        if (request.Subjects.Count > 0)
        {
            existingClass.Subjects = request.Subjects;
        }
        if (request.TeacherIds.Count > 0)
        {
            existingClass.TeacherIds = request.TeacherIds;
        }
    }
}

using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Domain.Entities.JunctionEntities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Application.Features.SchoolClassManagement.Commands.UnassignTeacherFromClass;

public class UnassignTeacherFromClassCommandHandler : IRequestHandler<UnassignTeacherFromClassCommand>
{
    private readonly ITeacherRepository _teacherRepository;
    public UnassignTeacherFromClassCommandHandler(ITeacherRepository teacherRepository)
    {
        _teacherRepository = teacherRepository;
    }

    public async Task Handle(UnassignTeacherFromClassCommand request, CancellationToken cancellationToken)
    {
        Expression<Func<TeacherClassAssignment, bool>> filter = 
            t => t.TeacherId == request.TeacherId
                 && t.ClassId == request.ClassId;
        var assignment = await _teacherRepository.GetItemByConditionAsync<TeacherClassAssignment>(filter);
        if (assignment is null)
        {
            throw new NotFoundException("Not Found");
        }
        var isDeleted = await _teacherRepository.DeleteByIdAsync<TeacherClassAssignment>(assignment.Id);
        if (!isDeleted)
        {
            throw new Exception("Failed to unassign teacher from class.");
        }
    }
}

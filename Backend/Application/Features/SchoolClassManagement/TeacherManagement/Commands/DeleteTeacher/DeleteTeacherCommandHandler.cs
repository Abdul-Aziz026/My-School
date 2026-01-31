using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Features.SchoolClassManagement.TeacherManagement.Commands.DeleteTeacher;

public class DeleteTeacherCommandHandler : IRequestHandler<DeleteTeacherCommand>
{
    private readonly ITeacherRepository _teacherRepository;
    public DeleteTeacherCommandHandler(ITeacherRepository teacherRepository)
    {
        _teacherRepository = teacherRepository;
    }
    public async Task Handle(DeleteTeacherCommand request, CancellationToken cancellationToken)
    {
        var teacher = await _teacherRepository.GetByIdAsync<Teacher>(request.TeacherId);
        if (teacher == null)
        {
            throw new NotFoundException("teacher not found");
        }
        teacher.IsDeleted = true;
        var updated = await _teacherRepository.UpdateAsync(teacher);
        if (!updated)
        {
            throw new Exception("Teacher deleted failed");
        }
    }
}

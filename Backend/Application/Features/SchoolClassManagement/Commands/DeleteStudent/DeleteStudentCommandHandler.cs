using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Features.SchoolClassManagement.Commands.DeleteStudent;

public class DeleteStudentCommandHandler : IRequestHandler<DeleteStudentCommand>
{
    private readonly IStudentRepository _studentRepository;
    public DeleteStudentCommandHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByIdAsync<Student>(request.Id);
        if (student is null)
        {
            throw new NotFoundException("student not found");
        }

        student.IsDeleted = true;
        bool updated = await _studentRepository.UpdateAsync(student);
        if (!updated)
        {
            throw new Exception("Student deletion failed");
        }
    }
}
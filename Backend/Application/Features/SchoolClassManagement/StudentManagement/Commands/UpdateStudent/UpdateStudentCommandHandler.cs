using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Features.SchoolClassManagement.StudentManagement.Commands.UpdateStudent;

public class UpdateStudentCommandHandler : IRequestHandler<UpdateStudentCommand>
{
    private readonly IStudentRepository _studentRepository;

    public UpdateStudentCommandHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByIdAsync<Student>(request.Id);
        if (student is null)
        {
            throw new InvalidOperationException("Student not found");
        }

        try
        {
            ApplyUpdateStudentRequest(request, student);
            await _studentRepository.UpdateAsync<Student>(student);
        }
        catch (Exception ex)
        {
            throw new Exception("Unknown Exception occur");
        }
    }

    private static void ApplyUpdateStudentRequest(UpdateStudentCommand request, Student student)
    {
        if (!string.IsNullOrWhiteSpace(request.ClassId))
        {
            student.ClassId = request.ClassId;
        }
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            student.Email = request.Email;
        }
        if (!string.IsNullOrWhiteSpace(request.Phone))
        {
            student.Phone = request.Phone;
        }
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            student.Name = request.Name;
        }
        if (!string.IsNullOrWhiteSpace(request.StudentNumber))
        {
            student.StudentNumber = request.StudentNumber;
        }
        if (request.DateOfBirth is not null)
        {
            student.DateOfBirth = request.DateOfBirth.Value;
        }
        if (request.Grade is not null)
        {
            student.Grade = request.Grade.Value;
        }
        if (request.Status is not null)
        {
            student.Status = request.Status.Value;
        }
    }
}
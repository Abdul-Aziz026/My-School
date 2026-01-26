
using Application.Common.Interfaces.Repositories;
using Application.Features.SchoolClassManagement.DTOs;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.SchoolClassManagement.Commands.CreateStudent;

public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, CreateStudentResponseDto>
{
    private readonly IStudentRepository _studentRepository;
    private readonly ILogger<CreateStudentCommandHandler> _logger;
    public CreateStudentCommandHandler(
            IStudentRepository studentRepository,
            ILogger<CreateStudentCommandHandler> logger)
    {
        _studentRepository = studentRepository;
        _logger = logger;
    }
    public async Task<CreateStudentResponseDto> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
    {
        var existingStudent = await _studentRepository.GetItemByConditionAsync<Student>(o => o.Email == request.Email);
        if (existingStudent != null)
        {
            throw new ArgumentException("Student with this email already exists");
        }
        try
        {
            // Create student (No transaction needed - single operation)
            var student = new Student
            {
                Id = Guid.NewGuid().ToString(),
                StudentNumber = request.StudentNumber,
                Name = request.Name,
                DateOfBirth = request.DateOfBirth,
                Email = request.Email,
                Phone = request.Phone,
                Grade = request.Grade,
                Section = request.Section,
                CreatedAt = DateTime.UtcNow,
                Status = StudentStatus.Active,
                SchoolId = request.SchoolId
            };
            await _studentRepository.AddAsync(student);
            _logger.LogInformation("Student created successfully: {StudentId}", student.Id);

            return new CreateStudentResponseDto
            {
                Success = true,
                Message = "Student created successfully",
                StudentId = student.Id,
                StudentNumber = student.StudentNumber
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create student: {Email}", request.Email);
            return new CreateStudentResponseDto
            {
                Success = false,
                Message = "Failed to create student. Please try again."
            };
        }
    }

    private string GenerateStudentNumber()
    {
        var year = DateTime.UtcNow.Year;
        var random = new Random().Next(1000, 9999);
        return $"STU{year}{random}";
    }
}
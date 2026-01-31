using Application.Features.SchoolClassManagement.StudentManagement.DTOs;
using MediatR;

namespace Application.Features.SchoolClassManagement.StudentManagement.Commands.CreateStudent;

public class CreateStudentCommand : IRequest<CreateStudentResponseDto>
{
    public string Name { get; set; }
    public string StudentNumber { get; init; } = default!;
    public string SchoolId { get; init; } = default!;

    public DateTime DateOfBirth { get; init; }

    public string Email { get; init; } = default!;
    public string Phone { get; init; } = default!;

    public int Grade { get; init; }
    public string Section { get; init; } = default!;
}



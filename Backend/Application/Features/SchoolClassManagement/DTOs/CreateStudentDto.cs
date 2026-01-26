using System.ComponentModel.DataAnnotations;
using Application.Features.SchoolClassManagement.Commands.CreateStudent;

namespace Application.Features.SchoolClassManagement.DTOs;

public sealed class CreateStudentDto
{
    [Required(ErrorMessage = "Name must be non empty")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Student number must be non empty")]
    public string StudentNumber { get; set; }
    public string SchoolId { get; set; }

    [Required(ErrorMessage = "Dateof birth must be non empty")]
    public DateTime DateOfBirth { get; init; }

    public string Email { get; init; } = default!;
    public string Phone { get; init; } = default!;

    public int Grade { get; init; }
    public string Section { get; init; } = default!;


    public CreateStudentCommand ToCreateStudentCommand()
    {
        return new CreateStudentCommand()
        {
            Name = this.Name,

        };
    }
}

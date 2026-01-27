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
    public DateTime DateOfBirth { get; set; }

    public string Email { get; set; }
    public string Phone { get; set; }

    public int Grade { get; set; }
    public string Section { get; set; }


    public CreateStudentCommand ToCreateStudentCommand()
    {
        return new CreateStudentCommand()
        {
            Name = this.Name,
            StudentNumber = this.StudentNumber,
            SchoolId = this.SchoolId,
            DateOfBirth = this.DateOfBirth,
            Email = this.Email,
            Phone = this.Phone,
            Grade = this.Grade,
            Section = this.Section
        };
    }
}

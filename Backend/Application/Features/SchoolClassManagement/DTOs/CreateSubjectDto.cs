
using Application.Features.SchoolClassManagement.Commands.CreateSubject;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.SchoolClassManagement.DTOs;

public class CreateSubjectDto
{
    [Required(ErrorMessage = "Subject name required")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Code required")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "Credits required")]
    public int Credits { get; set; }
    public string Description { get; set; } = string.Empty;

    public CreateSubjectCommand ToCreateSubjectCommand()
    {
        return new CreateSubjectCommand
        {
            Name = Name,
            Code = Code,
            Description = Description,
            Credits = Credits
        };
    }
}

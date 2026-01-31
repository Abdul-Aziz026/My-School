using Application.Features.SchoolClassManagement.SubjectManagement.Commands.UpdateSubject;
using Domain.Entities;

namespace Application.Features.SchoolClassManagement.SubjectManagement.DTOs;

public class UpdateSubjectDto
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Credits { get; set; }
    public bool IsActive { get; set; }

    public UpdateSubjectCommand ToUpdateSubjectCommand(string id)
    {
        return new UpdateSubjectCommand()
        {
            Id = id,
            Name = this.Name,
            Code = this.Code,
            Description = this.Description,
            IsActive = this.IsActive,
            Credits = this.Credits
        };
    }
}

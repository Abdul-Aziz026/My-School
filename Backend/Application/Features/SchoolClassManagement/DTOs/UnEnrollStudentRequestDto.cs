
using Application.Features.SchoolClassManagement.Commands.UnenrollStudent;

namespace Application.Features.SchoolClassManagement.DTOs;

public class UnEnrollStudentRequestDto
{
    public string StudentId { get; set; } = string.Empty;
    public string ClassId { get; set; } = string.Empty;
    public List<string> SubjectIds { get; set; } = new();

    public UnenrollStudentCommand ToUnenrollStudentCommand()
    {
        return new UnenrollStudentCommand
        {
            StudentId = StudentId,
            ClassId = ClassId,
            SubjectIds = SubjectIds
        };
    }
}

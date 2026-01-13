
using Application.Features.SchoolClassManagement.Commands.EnrollStudent;
using Application.Features.SchoolClassManagement.Commands.UnenrollStudent;

namespace Application.Features.SchoolClassManagement.DTOs;

public class EnrollStudentRequestDto
{
    public string StudentId { get; set; } = string.Empty;
    public string ClassId { get; set; } = string.Empty;
    public List<string> SubjectIds { get; set; } = new();

    public EnrollStudentCommand ToEnrollStudentCommand()
    {
        return new EnrollStudentCommand
        {
            StudentId = StudentId,
            ClassId = ClassId,
            SubjectIds = SubjectIds
        };
    }
}

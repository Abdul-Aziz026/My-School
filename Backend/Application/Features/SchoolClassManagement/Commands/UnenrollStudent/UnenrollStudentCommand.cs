
using MediatR;

namespace Application.Features.SchoolClassManagement.Commands.UnenrollStudent;

public class UnenrollStudentCommand : IRequest
{
    public string StudentId { get; set; } = string.Empty;
    public string ClassId { get; set; } = string.Empty;
}

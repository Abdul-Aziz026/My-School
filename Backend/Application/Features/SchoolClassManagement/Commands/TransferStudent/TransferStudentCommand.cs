
using MediatR;

namespace Application.Features.SchoolClassManagement.Commands.TransferStudent;

public class TransferStudentCommand : IRequest<string>
{
    public string StudentId { get; set; } = string.Empty;
    public string FromClassId { get; set; } = string.Empty;
    public string ToClassId { get; set; } = string.Empty;
}

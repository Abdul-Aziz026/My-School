

using Application.Features.SchoolClassManagement.Commands.TransferStudent;

namespace Application.Features.SchoolClassManagement.DTOs;

public class TransferStudentRequestDto
{
    public string StudentId { get; set; } = string.Empty;
    public string FromClassId { get; set; } = string.Empty;
    public string ToClassId {  get; set; } = string.Empty;
    public List<string> SubjectIds { get; set; } = new();

    public TransferStudentCommand ToTransferStudentCommand()
    {
        return new TransferStudentCommand
        {
            StudentId = StudentId,
            FromClassId = FromClassId,
            ToClassId = ToClassId,
            SubjectIds = SubjectIds
        };
    }
}


using MediatR;

namespace Application.Features.SchoolClassManagement.ExamManagement.Commands.PublishExam;

public class PublishExamCommand : IRequest
{
    public string ExamId { get; set; } = string.Empty;
    public PublishExamCommand(string examId)
    {
        ExamId = examId;
    }
}

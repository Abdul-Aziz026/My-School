
using Application.Features.SchoolClassManagement.StudentManagement.DTOs;
using MediatR;

namespace Application.Features.SchoolClassManagement.StudentManagement.Commands.SubmitExam;

public class SubmitExamCommand : IRequest<ExamResultDto>
{
    public string ExamId { get; set; } = string.Empty;
    public string StudentId { get; set; } = string.Empty;
    public Dictionary<string, string> Answers { get; set; } = new();
}

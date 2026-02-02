using MediatR;

namespace Application.Features.SchoolClassManagement.ExamManagement.Commands.CreateExam;

public class CreateExamCommand : IRequest<string>
{
    public string ClassId { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string ExamName { get; set; } = string.Empty;
    public string ExamType { get; set; } = string.Empty;
    public DateTime ExamDate { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int Duration { get; set; }
    public int TotalMarks { get; set; }
    public int PassingMarks { get; set; }
}
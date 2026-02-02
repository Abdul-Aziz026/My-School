using Application.Features.SchoolClassManagement.ExamManagement.Commands.CreateExam;

namespace Application.Features.SchoolClassManagement.ExamManagement.DTOs;

public class CreateExamDto
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

    public CreateExamCommand ToCreateExamCommand()
    {
        return new CreateExamCommand
        {
            ClassId = this.ClassId,
            SubjectName = this.SubjectName,
            ExamName = this.ExamName,
            ExamType = this.ExamType,
            ExamDate = this.ExamDate,
            StartTime = this.StartTime,
            EndTime = this.EndTime,
            Duration = this.Duration,
            TotalMarks = this.TotalMarks,
            PassingMarks = this.PassingMarks
        };
    }
}

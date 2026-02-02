using Application.Features.SchoolClassManagement.StudentManagement.Commands.SubmitExam;

namespace Application.Features.SchoolClassManagement.StudentManagement.DTOs;

public class SubmitExamDto
{
    public string ExamId { get; set; } = string.Empty;
    public string StudentId { get; set; } = string.Empty;
    public Dictionary<string, string> Answers { get; set; } = new();
    // key = QuestionId, value = student's chosen answer

    public SubmitExamCommand ToSubmitExamCommand()
    {
        return new SubmitExamCommand()
        {
            ExamId = this.ExamId,
            StudentId = this.StudentId,
            Answers = this.Answers
        };
    }
}

namespace Domain.Entities;

public class ClassStudentEnrollment : BaseEntity
{
    public string StudentId { get; set; } = string.Empty;
    public string ClassId { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    public EnrollMentStatus Status { get; set; }
}

public enum EnrollMentStatus
{
    Pending,
    InActive,
    Active
}
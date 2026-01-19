namespace Domain.Entities.JunctionEntities;

public class Enrollment : BaseEntity
{
    public string StudentId { get; set; } = string.Empty;
    public string ClassId { get; set; } = string.Empty;
    public DateTime EnrollmentDate { get; set; }
    public string AcademicYear { get; set; }
    public decimal TuitionFee { get; set; }
    public EnrollMentStatus Status { get; set; }
    public List<string> SubjectIds { get; set; } = new();

    public string GetId() => Id;
}

public enum EnrollMentStatus
{
    Enrolled,
    Completed,
    Dropped
}
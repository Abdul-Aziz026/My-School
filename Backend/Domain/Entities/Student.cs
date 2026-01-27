
namespace Domain.Entities;

public class Student : BaseEntity
{
    public string ClassId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string StudentNumber { get; set; } = string.Empty;
    public string SchoolId { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public int Grade { get; set; }
    public string Section {  get; set; } = string.Empty;
    public StudentStatus Status { get; set; }
    public bool IsDeleted { get; set; } = false;

    public string GetId() => Id;
}

public enum StudentStatus
{
    Active,
    Inactive,
    Graduated,
    Suspended
}

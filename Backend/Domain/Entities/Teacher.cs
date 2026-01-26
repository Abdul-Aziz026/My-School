
namespace Domain.Entities;

public class Teacher : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string SchoolId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string EmployeeNumber { get; set; } = string.Empty;
    public TeacherStatus Status { get; set; }
    public DateTime HireDate { get; set; }

    // Academic
    public string Department { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty; // e.g. Senior Teacher
    public List<string> ClassIds {  get; set; } = new();
    public List<string> SubjectIds {  get; set; } = new();
}

public enum TeacherStatus
{
    Active,
    Inactive,
    Retired,
    OnLeave
}
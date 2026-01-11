
namespace Domain.Entities;

public class Class : BaseEntity
{
    public string SchoolId { get; set; } // ID of the school
    public string Name { get; set; } = string.Empty; // e.g., "Grade 5 - A"
    public int Grade { get; set; } // 1, 2, 3 ... 12 (school grade)
    public string Section { get; set; } = string.Empty; // e.g., "A", "B"
    public string AcademicYear { get; set; } = string.Empty; // e.g., "2025-2026"
    public int Capacity { get; set; } // Max students allowed
    public List<string> Subjects { get; set; } = new();
    public List<string> TeacherIds { get; set; } = new();
    public List<string> StudentIds { get; set; } = new();
    public bool IsActive { get; set; } = true;
}

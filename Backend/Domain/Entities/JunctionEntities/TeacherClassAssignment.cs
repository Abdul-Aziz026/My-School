
namespace Domain.Entities.JunctionEntities;

public class TeacherClassAssignment : BaseEntity
{
    public string TeacherId { get; set; } = string.Empty;
    public string ClassId { get; set; } = string.Empty;
    public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
}


namespace Domain.Entities;

public class Attendance : BaseEntity
{
    public string StudentId { get; set; } = string.Empty;
    public string ClassId { get; set; } = string.Empty;
    public DateTime Date { get; set; } // The actual day
    public DateTime EntryTime { get; set; }
    public DateTime? ExitTime { get; set; }

    public bool IsPresent { get; set; }
}

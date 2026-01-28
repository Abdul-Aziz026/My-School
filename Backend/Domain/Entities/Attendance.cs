
namespace Domain.Entities;

public class Attendance : BaseEntity
{
    public string StudentId { get; set; } = string.Empty;
    public DateTime EntryTime { get; set; }
    public DateTime ExitTime { get; set; }
    public DateTime Date { get; set; }
    public bool IsPresent { get; set; }
}

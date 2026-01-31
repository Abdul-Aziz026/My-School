
namespace Domain.Entities;

public class TimeSlot : BaseEntity
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string SlotName { get; set; } // e.g., Period 1, Period 2
}
